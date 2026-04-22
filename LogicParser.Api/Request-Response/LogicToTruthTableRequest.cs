using System;
using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class LogicToTruthTableRequest
{
    [Required]
    public string Raw{get; set;} = string.Empty;
}
