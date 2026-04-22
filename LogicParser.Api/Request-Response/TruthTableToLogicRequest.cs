using System;
using System.ComponentModel.DataAnnotations;

namespace LogicParser.Api.Request_Response;

public class TruthTableToLogicRequest
{
    [Required]
    public int Variables{get; set;}
    [Required]
    public List<string> VariableNames{get; set;} = []; //Supposed to have {Variables} elements

    [Required]
    public List<List<bool>> Values{get; set;} = []; //Supposed to have {Variables} + 1(result) columns and rows
}
