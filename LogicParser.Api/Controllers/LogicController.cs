using LogicParser.Api.Request_Response;
using LogicParser.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LogicParser.Api.SideEntities;

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
            var solved = await _service.Normalize(request.Raw);
            switch (solved.Problem)
            {
                case ProblemInput.Parenthesis:
                    return BadRequest(new {message = "Your parenthesis is wrong"});
                case ProblemInput.TooManyLetter:
                    return BadRequest(new {message = "Each proposition mustn't have more than 1 letter representing them"});
                case ProblemInput.TooManyOperation:
                    return BadRequest(new {message = "Apart from negative operation, all other operations can only appear between propositions"});
                case ProblemInput.Nothing:
                    return BadRequest(new {message = "Request only contain white space or empty"});
                case ProblemInput.WrongOperation:
                    return BadRequest(new {message = "Request contain some wrong operation"});
                case ProblemInput.StartOrEndWithBinaryOperation:
                    return BadRequest(new  {message = "Request can not start or end with a binary operation"});
            }
            var result = await _service.LogicToTruthTable(solved.String);
            if (result == null) return BadRequest(new {message = "Request is in wrong format"});
            return Ok(result);
        }
        [HttpPost("/truth-to-logic")]
        public async Task<IActionResult> TruthTableToLogic([FromBody] TruthTableToLogicRequest request)
        {
            var result = await _service.Solve(request);
            return Ok(result);
        }
    }
}
