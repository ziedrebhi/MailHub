using MailHub.Application.EmailQueue.Commands;
using MailHub.Application.EmailQueue.Queries;
using MailHub.Domain.Enums;
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
        /// <summary>
        /// Get All Email Queue Status
        /// </summary>
        /// <param name="status">Email Status</param>
        /// <returns>List of Email Queue</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllEmailQueueStatus([FromQuery] EmailStatus? status)
        {
            var result = await _mediator.Send(new GetAllEmailQueueStatusQuery(status));

            return Ok(result);
        }

        /// <summary>
        /// Get Email Queue Status
        /// </summary>
        /// <param name="id">Id of Email queue</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailQueueStatus(int id)
        {
            var result = await _mediator.Send(new GetEmailQueueStatusQuery(id));

            return Ok(result);
        }
    }
}