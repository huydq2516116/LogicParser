using System;

namespace LogicParser.Api.Request_Response;

public class TruthTableToLogicResponse
{
    public string Answer{get; set;} = string.Empty; //For example: A∧B∨C (which means (A∧B)∨C)
}
