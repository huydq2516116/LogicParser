

using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.SideEntities;

public class KnowledgeBase
{
    public Guid Id{get; set;} = Guid.NewGuid();
    [Required]
    public string Knowledge{get; set;} = string.Empty;
    [Required]
    public bool Value{get; set;}
}
