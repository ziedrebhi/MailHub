using MailHub.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MailHub.Api.Controllers
{
    /// <summary>
    /// Auth Controller for Users management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Sign-up for new users
        /// </summary>
        /// <param name="command"> User informations</param>
        /// <returns>User Id</returns>
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }
        /// <summary>
        /// Login to get Token
        /// </summary>
        /// <param name="command">Email and Password</param>
        /// <returns>JWT Token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }
    }
}
