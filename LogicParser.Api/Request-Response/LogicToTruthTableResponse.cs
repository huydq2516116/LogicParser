using System;
using LogicParser.Api.SideEntities;

namespace LogicParser.Api.Request_Response;

public class LogicToTruthTableResponse
{
    public int ColumnCount()
    {
        if (Result.Count > 0) return Result[0].Count;
        return 0;
    } 
    public int RowCount => Result.Count;
    public List<List<bool>> Result{get; set;} = [];
    
}
