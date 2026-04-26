using LogicParser.Api.Request_Response;
namespace LogicParser.Api.Services;

public class LogicService
{
    public async Task<List<string>?> Tokenize(string raw)
    {
        //AB && CD || EF -> GH

        List<string> operations = ["<=>", "->", "&&", "||", "-", "(", ")"];
        var result = new List<string>();
        var current = string.Empty;
        int i = 0;
        while (i < raw.Length)
        {
            var letter = raw[i];
            if (char.IsLetter(letter) || char.IsWhiteSpace(letter))
            {
                current += letter;
                i++;
                continue;
            }
            current = current.Trim();
            if (current != string.Empty) result.Add(current);

            current = string.Empty;

            var matchedOp = operations
            .FirstOrDefault(op => raw.AsSpan(i).StartsWith(op));

            if (matchedOp == null) return null;
            i += matchedOp.Length;
            result.Add(matchedOp);
        }

        current = current.Trim();
        if (current != string.Empty) result.Add(current);


        foreach (var s in result)
        {
            Console.Write($"_{s}_ ");
        }

        return result;
    }
    public async Task<LogicToTruthTableResponse?> LogicToTruthTable(List<string> tokens)
    {
        var stack = new Stack<string>();
        var queue = new Queue<string>();
        foreach (var token in tokens)
        {
            if (char.IsLetter(token[0]))
            {
                queue.Enqueue(token);
                if (stack.Count > 0 && stack.Peek() == "-") queue.Enqueue(stack.Pop());
            }
            else
            {
                if (stack.TryPeek(out var peek))
                    if (LogicStatic.Level(token) >= LogicStatic.Level(peek) && peek != "(")
                        queue.Enqueue(stack.Pop());
                stack.Push(token);
                if (token == ")")
                {
                    for (int i = stack.Count - 1; i >= 0; i--)
                    {
                        var pop = stack.Pop();
                        if (pop == "(") break;
                        if (pop != ")") queue.Enqueue(pop);
                    }
                    if (stack.Count > 0 && stack.Peek() == "-") queue.Enqueue(stack.Pop());
                }
            }
        }
        while (stack.Count > 0)
        {
            queue.Enqueue(stack.Pop());
        }

        for (int i = stack.Count - 1; i >= 0; i--)
        {
            var pop = stack.Pop();
            if (pop != ")" && pop != "(") queue.Enqueue(pop);
        }
        if (queue.Count <= 0) return null;
        Console.Write("Shunting yard result: ");
        foreach (var letter in queue)
        {
            Console.Write($"_{letter}_");
        }
        Console.Write("\n");
        var dict = new Dictionary<string, bool>();
        foreach (var letter in queue)
        {
            if (letter == "T" || letter == "F") continue;
            if (char.IsLetter(letter[0])) dict.TryAdd(letter, false);
        }
        var keyList = dict.Keys.ToList();
        keyList = [.. keyList.OrderByDescending(key => key)];



        var cellTable = new List<List<bool>>();

        while (true)
        {
            var sol = LogicStatic.Solution(queue, dict);

            bool found = false;
            foreach (var key in keyList)
            {
                if (dict[key] == false)
                {
                    found = true;
                    dict[key] = true;
                    foreach (var k in keyList)
                    {
                        if (k == key) break;
                        dict[k] = false;
                    }
                    break;
                }
            }
            if (sol == null) return null;
            cellTable.Add(sol);
            if (!found) break;
        }

        keyList.Reverse();
        var result = new LogicToTruthTableResponse
        {
            Result = cellTable,
            Prepositions = keyList
        };


        return result;
    }
    
    public async Task<TruthTableToLogicResponse> TruthTableToLogic(TruthTableToLogicRequest request)
    {
        int n = request.Variables;
        var names = request.VariableNames;

        // 1. Lấy minterms
        var minterms = new List<int>();

        for (int i = 0; i < request.Values.Count; i++)
        {
            var row = request.Values[i];
            bool result = row[n];

            if (result)
            {
                int value = 0;
                for (int j = 0; j < n; j++)
                {
                    if (row[j])
                        value |= 1 << (n - j - 1);
                }
                minterms.Add(value);
            }
        }

        // Edge cases
        if (minterms.Count == 0)
            return new TruthTableToLogicResponse { Answer = "0" };

        if (minterms.Count == (1 << n))
            return new TruthTableToLogicResponse { Answer = "1" };

        // 2. Quine–McCluskey
        var primeImplicants = LogicStatic.GetPrimeImplicants(minterms, n);

        // 3. Chọn implicants tối thiểu (greedy đủ dùng)
        var selected = LogicStatic.SelectImplicants(primeImplicants, minterms);

        // 4. Convert sang biểu thức
        string expression = string.Join("∨", selected.Select(p => LogicStatic.ToExpression(p, names)));

        return new TruthTableToLogicResponse
        {
            Answer = expression
        };
    }

    

}
