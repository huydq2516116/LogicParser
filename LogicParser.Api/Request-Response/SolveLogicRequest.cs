using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class SolveLogicRequest: IValidatableObject
{
    [Required]
    public List<string> Statements{get; set;} = [];
    [Required]
    public Dictionary<string,bool> KnowledgeBase{get; set;} = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Check duplicate statements
        if (Statements != null)
        {
            var duplicates = Statements
                .GroupBy(s => s)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                yield return new ValidationResult(
                    $"Statements bị trùng: {string.Join(", ", duplicates)}",
                    [nameof(Statements)]
                );
            }
        }

        // Optional: check duplicate keys ignoring case
        if (KnowledgeBase != null)
        {
            var duplicates = KnowledgeBase.Keys
                .GroupBy(k => k.ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                yield return new ValidationResult(
                    $"KnowledgeBase có key trùng (ignore case): {string.Join(", ", duplicates)}",
                    [nameof(KnowledgeBase)]
                );
            }
        }
    }
}
