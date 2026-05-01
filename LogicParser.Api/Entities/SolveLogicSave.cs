using System;

namespace LogicParser.Api.Entities;

public class SolveLogicSave
{
    public Guid Id{get; set;} = Guid.NewGuid();
    public int VariableCount{get; set;} 
    public int ClauseCount{get; set;}
    public string Variables{get; set;} = string.Empty; // A-B-C
}
