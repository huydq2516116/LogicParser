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

    

}
