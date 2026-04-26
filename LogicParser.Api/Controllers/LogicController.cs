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
            var tokens = await _service.Tokenize(request.Raw);
            if (tokens == null)
            {
                return BadRequest(new {message = "Request contain some unknown operations"});
            }
            var result = await _service.LogicToTruthTable(tokens);
            if (result == null) return BadRequest(new {message = "Request is in wrong format"});
            return Ok(result);
        }
        [HttpPost("/truth-to-logic")]
        public async Task<IActionResult> TruthTableToLogic([FromBody] TruthTableToLogicRequest request)
        {
            var result = await _service.TruthTableToLogic(request);
            return Ok(result);
        }
    }
}
