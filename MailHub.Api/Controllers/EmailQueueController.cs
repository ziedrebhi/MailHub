using MailHub.Application.EmailQueue.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailHub.Api.Controllers
{
    /// <summary>
    /// Email Queue management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailQueueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailQueueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Add Email To Queue
        /// </summary>
        /// <param name="command">Recipient , Template Id and parameters </param>
        /// <returns>Message</returns>
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddEmailToQueue([FromBody] AddEmailToQueueCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(new { message = "Email added to queue successfully." });
            }

            return BadRequest(new { message = "Failed to add email to queue." });
        }
    }
}