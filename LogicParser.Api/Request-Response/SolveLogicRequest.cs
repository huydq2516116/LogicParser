using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class SolveLogicRequest
{
    [Required]
    public List<string> Statements{get; set;} = [];
    [Required]
    public Dictionary<string,bool> KnowledgeBase{get; set;} = [];
}
