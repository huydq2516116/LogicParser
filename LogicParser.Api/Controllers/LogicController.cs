using LogicParser.Api.Request_Response;
using LogicParser.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicParser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogicController(LogicService service) : ControllerBase
    {
        public readonly LogicService _service = service;
        [HttpPost("/logic-to-truth")]
        public async Task<IActionResult> LogicToTruthTable([FromBody] LogicToTruthTableRequest request)
        {
            var result = await _service.LogicToTruthTable(request.Raw);
            if (result == null) return BadRequest(new {message = "Request is in wrong format"});
            return Ok(result);
        }
        [HttpPost("/truth-to-logic")]
        public async Task<IActionResult> TruthTableToLogic([FromBody] TruthTableToLogicRequest request)
        {
            var result = await _service.TruthTableToLogic(request);
            return Ok(result);
        }
        [HttpPost("/solve-logic")]
        public async Task<IActionResult> SolveLogic([FromBody] SolveLogicRequest request)
        {
            var statements = new List<string>();
            foreach(var s in request.Statements)
            {
                var tmp = s.Trim();
                if (tmp == string.Empty) return BadRequest(new {message  = "Statement can not be empty"});
                if (tmp == "T" || tmp == "F") return BadRequest(new {message  = $"Statement can not be {tmp}"});
                statements.Add(tmp);
            }
            var result = await _service.SolveLogic(statements, request.KnowledgeBase);
            if (result == null) return BadRequest(new {message = "Wrong Format"});
            return Ok(result);
        }
    }
}
