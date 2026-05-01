using System.ComponentModel.DataAnnotations;
using LogicParser.Api.SideEntities;

namespace LogicParser.Api.Request_Response;

public class SolveLogicRequest: IValidatableObject
{
    [Required]
    public List<string> Statements{get; set;} = [];
    [Required]
    public List<KnowledgeBase> KnowledgeBase{get; set;} = [];

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
        
    }
}


