using LogicParser.Api.Request_Response;
using LogicParser.Api.SideEntities;
using Microsoft.VisualBasic;
namespace LogicParser.Api.Services;

public class LogicService
{

    public async Task<NormalizedString> Normalize(string raw)
    {

        List<char> operations = ['-', '&', '|', '>', '=', '(', ')'];
        string result = string.Empty;
        string current = string.Empty;
        foreach (var letter in raw)
        {
            if (operations.Contains(letter))
            {
                current = current.Trim();
                result += current;
                current = string.Empty;
                result += letter;
            }
            else
            {
                current += letter;
            }
        }
        current = current.Trim();
        result += current;
        var problem = ProblemInput.Normal;
        int open = 0;
        bool previousLetter = false;
        bool previousOp = false;

        foreach(var (index,letter) in result.Index())
        {
            if (!char.IsLetter(letter) && !operations.Contains(letter))
            {
                problem = ProblemInput.WrongOperation;
                break;
            }
            if (letter == '(')  open += 1;
        

            
            if (letter == ')') open -= 1;
            if (open < 0)
            {
                problem = ProblemInput.Parenthesis;
                break;
            }

            if (char.IsLetter(letter))
            {
                if (!previousLetter) previousLetter = true;
                else
                {
                    problem = ProblemInput.TooManyLetter;
                    break;
                }
            }

            if (operations[1..5].Contains(letter))
            {
                if (index > 0 && result[index-1] == '(')
                {
                    problem = ProblemInput.StartOrEndWithBinaryOperation;
                    break;
                }
                previousLetter = false;
                if (!previousOp) previousOp = true;
                else
                {
                    problem = ProblemInput.TooManyOperation;
                    break;
                }
            }
            else
            {
                previousOp = false;
            }
        }

        if (problem == ProblemInput.Normal && open != 0) problem = ProblemInput.Parenthesis;

        if (string.IsNullOrWhiteSpace(result)) return new NormalizedString{Problem = ProblemInput.Nothing};
        

        if (operations[1..5].Contains(result[0]) || operations[0..5].Contains(result[^1])) problem = ProblemInput.StartOrEndWithBinaryOperation;
        Console.WriteLine($"Normalize Result: {result}");
        return new NormalizedString
        {
            String = result,
            Problem = problem
        };
    }
    public async Task<LogicToTruthTableResponse?> LogicToTruthTable(string solved)
    {
        
        //&A&B

        var stack = new Stack<char>();
        var queue = new Queue<char>();

        foreach (var letter in solved)
        {
            if (char.IsLetter(letter))
            {
                queue.Enqueue(letter);
                if (stack.Count > 0 && stack.Peek() == '-')
                {
                    queue.Enqueue(stack.Pop());
                }
            }
            else if (letter == '-')
            {
                stack.Push(letter);
            }
            else
            {
                if (stack.Count > 0 && Level(letter) <= Level(stack.Peek()) && letter != '(' && letter != ')')
                { queue.Enqueue(stack.Pop()); }

                stack.Push(letter);
                if (letter == ')')
                {
                    for (int i = stack.Count - 1; i >= 0; i--)
                    {
                        var pop = stack.Pop();
                        if (pop == '(') break;
                        if (pop != ')') queue.Enqueue(pop);
                    }
                    if (stack.Count > 0 && stack.Peek() == '-') queue.Enqueue(stack.Pop());
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
            if (pop != ')' && pop != '(') queue.Enqueue(pop);
        }
        if (queue.Count <= 0) return null;
        Console.Write("Shunting yard result: ");
        foreach (var letter in queue)
        {
            Console.Write(letter);
        }
        Console.Write("\n");
        var dict = new Dictionary<char, bool>();
        foreach (var letter in queue)
        {
            if (char.IsLetter(letter)) dict.TryAdd(letter, false);
        }
        var keyList = dict.Keys.ToList();
        keyList = [.. keyList.OrderByDescending(key => key)];

        var cellTable = new List<List<bool>>();

        while (true)
        {
            var sol = Solution(queue, dict);
            
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
            if (sol == null) continue;
            cellTable.Add(sol);
            if (!found) break;
        }


        var result = new LogicToTruthTableResponse
        {
            Result = cellTable,
        };


        return result;
    }
    static List<bool>? Solution(Queue<char> solved, Dictionary<char, bool> dict)
    {
        bool valueTest;
        bool foundT = dict.TryGetValue('T', out valueTest);
        if (foundT && valueTest != true) return null;
        bool foundF = dict.TryGetValue('F', out valueTest);
        if (foundF && valueTest != false) return null;

        var stack = new Stack<bool>();
        foreach (var letter in solved)
        {
            if (char.IsLetter(letter))
            {
                bool tryGetResult = dict.TryGetValue(letter, out bool value);
                if (tryGetResult) stack.Push(value);
                continue;
            }
            bool first, second;
            switch (letter)
            {
                case '-':
                    second = stack.Pop();
                    stack.Push(!second);
                    break;
                case '&':
                    second = stack.Pop();
                    first = stack.Pop();
                    if (second && first) stack.Push(true);
                    else stack.Push(false);
                    break;
                case '|':
                    second = stack.Pop();
                    first = stack.Pop();
                    if (second || first) stack.Push(true);
                    else stack.Push(false);
                    break;
                case '>':
                    second = stack.Pop();
                    first = stack.Pop();
                    if (second || !first) stack.Push(true);
                    else stack.Push(false);
                    break;
                case '=':
                    second = stack.Pop();
                    first = stack.Pop();
                    if (second == first) stack.Push(true);
                    else stack.Push(false);
                    break;
            }

        }
        var result = new List<bool>();
        foreach (var pair in dict)
        {
            var cell = pair.Value;
            result.Add(cell);
        }
        var sol = stack.Pop();
        result.Add(sol);
        if (stack.Count != 0) Console.WriteLine("Stack is still left, somehow");
        return result;
    }
    static int Level(char operation)
    {
        return operation switch
        {
            '&' => 1,
            '|' => 2,
            '>' => 3,
            '=' => 4,
            _ => 0,
        };
    }




    public async Task<TruthTableToLogicResponse> Solve(TruthTableToLogicRequest request)
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
        var primeImplicants = GetPrimeImplicants(minterms, n);

        // 3. Chọn implicants tối thiểu (greedy đủ dùng)
        var selected = SelectImplicants(primeImplicants, minterms);

        // 4. Convert sang biểu thức
        string expression = string.Join("∨", selected.Select(p => ToExpression(p, names)));

        return new TruthTableToLogicResponse
        {
            Answer = expression
        };
    }

    private static List<string> GetPrimeImplicants(List<int> minterms, int n)
    {
        var groups = new Dictionary<int, List<string>>();

        foreach (var m in minterms)
        {
            string bin = Convert.ToString(m, 2).PadLeft(n, '0');
            int ones = bin.Count(c => c == '1');

            if (!groups.ContainsKey(ones))
                groups[ones] = new List<string>();

            groups[ones].Add(bin);
        }

        var primeImplicants = new HashSet<string>();
        bool canCombine = true;

        while (canCombine)
        {
            canCombine = false;
            var newGroups = new Dictionary<int, List<string>>();
            var used = new HashSet<string>();

            var keys = groups.Keys.OrderBy(x => x).ToList();

            for (int i = 0; i < keys.Count - 1; i++)
            {
                foreach (var a in groups[keys[i]])
                {
                    foreach (var b in groups[keys[i + 1]])
                    {
                        int diff = 0;
                        int pos = -1;

                        for (int k = 0; k < a.Length; k++)
                        {
                            if (a[k] != b[k])
                            {
                                diff++;
                                pos = k;
                            }
                        }

                        if (diff == 1)
                        {
                            canCombine = true;
                            used.Add(a);
                            used.Add(b);

                            var combined = a.Substring(0, pos) + "-" + a.Substring(pos + 1);

                            int ones = combined.Count(c => c == '1');

                            if (!newGroups.ContainsKey(ones))
                                newGroups[ones] = new List<string>();

                            if (!newGroups[ones].Contains(combined))
                                newGroups[ones].Add(combined);
                        }
                    }
                }
            }

            foreach (var group in groups.Values)
            {
                foreach (var item in group)
                {
                    if (!used.Contains(item))
                        primeImplicants.Add(item);
                }
            }

            groups = newGroups;
        }

        return primeImplicants.ToList();
    }

    private static List<string> SelectImplicants(List<string> implicants, List<int> minterms)
    {
        var covered = new HashSet<int>();
        var result = new List<string>();

        while (covered.Count < minterms.Count)
        {
            string? best = null;
            int bestCover = -1;

            foreach (var imp in implicants)
            {
                int count = 0;

                foreach (var m in minterms)
                {
                    if (!covered.Contains(m) && Covers(imp, m))
                        count++;
                }

                if (count > bestCover)
                {
                    bestCover = count;
                    best = imp;
                }
            }

            if (best == null)
                break;

            result.Add(best);

            foreach (var m in minterms)
            {
                if (Covers(best, m))
                    covered.Add(m);
            }
        }

        return result;
    }

    private static bool Covers(string implicant, int minterm)
    {
        string bin = Convert.ToString(minterm, 2).PadLeft(implicant.Length, '0');

        for (int i = 0; i < implicant.Length; i++)
        {
            if (implicant[i] == '-')
                continue;

            if (implicant[i] != bin[i])
                return false;
        }

        return true;
    }

    private static string ToExpression(string pattern, List<string> names)
    {
        var parts = new List<string>();

        for (int i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] == '-')
                continue;

            if (pattern[i] == '1')
                parts.Add(names[i]);
            else
                parts.Add($"¬{names[i]}");
        }

        if (parts.Count == 0)
            return "1";

        return string.Join("∧", parts);
    }

}
