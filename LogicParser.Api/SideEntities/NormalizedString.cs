using System;

namespace LogicParser.Api.SideEntities;

public class NormalizedString
{
    public string String{get; set;} = string.Empty;
    public ProblemInput Problem{get; set;} = ProblemInput.Normal;
}

public enum ProblemInput
{
    Normal,
    Parenthesis,
    TooManyLetter,
    TooManyOperation,
    Nothing,
    WrongOperation,
    StartOrEndWithBinaryOperation,
}