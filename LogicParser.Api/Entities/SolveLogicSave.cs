using System;
using LogicParser.Api.Request_Response;
using LogicParser.Api.SideEntities;

namespace LogicParser.Api.Entities;

public class SolveLogicSave
{
    public Guid Id{get; set;} = Guid.NewGuid();
    public Guid UserId{get; set;}
    public List<string> Variables{get; set;} = []; 
    public List<KnowledgeBase> Clauses{get; set;} = []; 
    public List<string> Result{get; set;} = []; 
}