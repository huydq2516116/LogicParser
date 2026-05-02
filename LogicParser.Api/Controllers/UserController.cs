using LogicParser.Api.Request_Response;
using LogicParser.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogicParser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService service) : ControllerBase
    {
        public readonly UserService _service = service;
        [HttpPost("/sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var result = await _service.SignUp(request.Username, request.Password);
            if (result == null) return BadRequest(new {message = "This username is already in used"});
            return Ok(result);
        }
        [HttpPost("/sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignUpRequest request)
        {
            var result = await _service.SignIn(request.Username, request.Password);
            if (result == null) return BadRequest(new {message = "Username is not available or password is wrong"});
            return Ok(result);
        }
    }
}
