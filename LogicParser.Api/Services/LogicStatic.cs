using System;

namespace LogicParser.Api.Services;

public static class LogicStatic
{
    public static List<string>? Tokenize(string raw)
    {
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
    public static Queue<string>? ShuntingYard(List<string> tokens)
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
        return queue;
    }
    public static List<string>? PostShuntingYard(List<List<bool>> cellTable, Queue<string> queue)
    {
        var dict = new Dictionary<string, bool>();
        foreach (var letter in queue)
        {
            if (letter == "T" || letter == "F") continue;
            if (char.IsLetter(letter[0])) dict.TryAdd(letter, false);
        }
        var keyList = dict.Keys.ToList();
        keyList = [.. keyList.OrderByDescending(key => key)];
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
            if (sol == null) return null;
            cellTable.Add(sol);
            if (!found) break;
        }

        keyList.Reverse();
        return keyList;
    }
    public static List<bool>? Solution(Queue<string> solved, Dictionary<string, bool> dict)
    {

        var stack = new Stack<bool>();
        foreach (var letter in solved)
        {
            if (char.IsLetter(letter[0]))
            {
                if (letter == "T") stack.Push(true);
                else if (letter == "F") stack.Push(false);
                else
                {
                    bool tryGetResult = dict.TryGetValue(letter, out bool value);
                    if (tryGetResult) stack.Push(value);
                }
                continue;
            }
            bool first, second;
            switch (letter)
            {
                case "-":
                    if (stack.TryPop(out second))
                    {
                        stack.Push(!second);
                        break;
                    }
                    return null;
                case "&&":
                    if (stack.TryPop(out second) && stack.TryPop(out first))
                    {
                        stack.Push(first && second);
                        break;
                    }
                    return null;
                case "||":
                    if (stack.TryPop(out second) && stack.TryPop(out first))
                    {
                        stack.Push(first || second);
                        break;
                    }
                    return null;
                case "->":
                    if (stack.TryPop(out second) && stack.TryPop(out first))
                    {
                        stack.Push(!first || second);
                        break;
                    }
                    return null;
                case "<=>":
                    if (stack.TryPop(out second) && stack.TryPop(out first))
                    {
                        stack.Push(first == second);
                        break;
                    }
                    return null;
                default:
                    return null;
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
        if (stack.Count != 0)
        {
            Console.WriteLine("Stack is still left");
            return null;
        }

        return result;
    }
    static int Level(string operation)
    {
        return operation switch
        {
            "-" => 1,
            "&&" => 2,
            "||" => 3,
            "->" => 4,
            "<=>" => 5,
            _ => 0,
        };
    }
    public static List<string> GetPrimeImplicants(List<int> minterms, int n)
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
    public static List<string> SelectImplicants(List<string> implicants, List<int> minterms)
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
    public static bool Covers(string implicant, int minterm)
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
    public static string ToExpression(string pattern, List<string> names)
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
