using MailHub.Application.EmailConfiguration.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> SetEmailConfiguration([FromBody] SetEmailConfigurationCommand command)
        {
            var result = await _mediator.Send(command);

            if (result)
                return Ok("Email configuration saved successfully.");
            else
                return BadRequest("Failed to set email configuration."); // Failure
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmailConfiguration(int id, [FromBody] SetEmailConfigurationCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (result)
                return Ok("Email configuration updated successfully.");
            else
                return BadRequest("Failed to update email configuration."); // Failure
        }
    }
}
