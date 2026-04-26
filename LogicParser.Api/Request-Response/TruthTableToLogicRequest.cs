using System;
using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class TruthTableToLogicRequest:IValidatableObject
{
    [Required]
    public int Variables{get; set;}
    [Required]
    public List<string> VariableNames{get; set;} = []; //Supposed to have {Variables} elements

    [Required]
    public List<List<bool>> Values{get; set;} = []; //Supposed to have {Variables} + 1(result) columns, {2 ** Variables} rows
    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        // 1. Check VariableNames count
        if (VariableNames.Count != Variables)
        {
            yield return new ValidationResult(
                $"VariableNames phải có đúng {Variables} phần tử",
                [nameof(VariableNames)]);
        }

        // 2. Check duplicate VariableNames
        if (VariableNames.Count != VariableNames.Distinct().Count())
        {
            yield return new ValidationResult(
                "VariableNames không được trùng",
                [nameof(VariableNames)]);
        }

        // 3. Check Values column count
        foreach (var row in Values)
        {
            if (row.Count != Variables + 1)
            {
                yield return new ValidationResult(
                    $"Mỗi row trong Values phải có {Variables + 1} cột",
                    [nameof(Values)]);
                break;
            }
        }

        // 4. Check duplicate rows (chỉ xét Variables cột đầu)
        var seen = new HashSet<string>();

        foreach (var row in Values)
        {
            var key = string.Join(",", row.Take(Variables));

            if (!seen.Add(key))
            {
                yield return new ValidationResult(
                    "Values có row bị trùng ở phần biến đầu vào",
                    [nameof(Values)]);
                break;
            }
        }

        // 5. (optional) Check số lượng dòng đúng = 2^Variables
        int expectedRows = 1 << Variables;
        if (Values.Count != expectedRows)
        {
            yield return new ValidationResult(
                $"Values nên có {expectedRows} dòng (2^{Variables})",
                [nameof(Values)]);
        }
    }
}
