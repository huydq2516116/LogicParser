using LogicParser.Api.Request_Response;
namespace LogicParser.Api.Services;

public class LogicService
{
    public async Task<LogicToTruthTableResponse?> LogicToTruthTable(string raw)
    {
        var tokens = LogicStatic.Tokenize(raw);
        if (tokens == null) return null;

        var queue = LogicStatic.ShuntingYard(tokens);
        if (queue == null) return null;
        Console.Write("Shunting yard result: ");
        foreach (var letter in queue)
        {
            Console.Write($"_{letter}_");
        }
        Console.Write("\n");
        var cellTable = new List<List<bool>>();
        var keyList = LogicStatic.PostShuntingYard(cellTable, queue);
        if (keyList == null) return null;
        return new LogicToTruthTableResponse
        {
            Result = cellTable,
            Prepositions = keyList
        };
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
    public async Task<SolveLogicResponse?> SolveLogic(List<string> statements, Dictionary<string, bool> knowledgeBase)
    {
        var list = new List<List<bool>>();
        var dict = new Dictionary<string, bool>();
        foreach (var s in statements)
        {
            var tryAddResult = dict.TryAdd(s, false);
            if (!tryAddResult) return null;
        }
        var keyList = dict.Keys.ToList();
        keyList = [.. keyList.OrderByDescending(key => key)];
        
        var boolList = new List<bool>();
        for (int i=0; i<statements.Count; i++) boolList.Add(false);
        while (true)
        {
            var tmpList = new List<bool>();
            for (int i=0; i<statements.Count; i++) tmpList.Add(boolList[i]);
            list.Add(tmpList);
            bool found = false;
            for (int i=0; i<statements.Count; i++)
            {
                if (!boolList[i])
                {
                    found = true;
                    boolList[i] = true;
                    for (int j = i-1; j>=0; j--)  boolList[j] = false;
                    break;
                }
            }
            if (!found) break;
        }


        foreach (var pair in knowledgeBase)
        {
            var tokens = LogicStatic.Tokenize(pair.Key);
            if (tokens == null) return null;
            var queue = LogicStatic.ShuntingYard(tokens);
            if (queue == null) return null;

            var tmpList = new List<List<bool>>();
            foreach (var lb in list)
            {
                if (dict.Count != lb.Count) return null;
                for(int i=0; i<statements.Count; i++)
                {
                    dict[keyList[i]] = lb[i];
                }
                var sol = LogicStatic.Solution(queue, dict);
                if (sol == null) return null;
                if (sol.LastOrDefault() == pair.Value)
                {
                    Console.WriteLine($"Testing At {pair.Key}:");
                    foreach(var kvp in dict)
                    {
                        Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                    }
                    tmpList.Add(lb);
                }
            }
            list = tmpList;
        }
        return new SolveLogicResponse
        {
            Result = list,
            Prepositions = keyList,
        };
    }



}
