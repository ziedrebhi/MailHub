using MailHub.Application.EmailTemplates.Commands;
using MailHub.Application.EmailTemplates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailHub.Api.Controllers
{
    /// <summary>
    /// Email Templates management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Email Template by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Template</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var query = new GetTemplateByIdQuery { Id = id };
            var template = await _mediator.Send(query);

            if (template == null)
                return NotFound(); // Return 404 if template not found

            return Ok(template); // Return 200 with the template
        }

        /// <summary>
        /// Create a new template
        /// </summary>
        /// <param name="command">Email template informations</param>
        /// <returns>Template Id</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateCommand command)
        {
            var templateId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetTemplate), new { id = templateId }, null); // Return 201 created
        }

        /// <summary>
        ///  Update an existing template
        /// </summary>
        /// <param name="id">template id</param>
        /// <param name="command">Email template informations</param>
        /// <returns>Message</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateCommand command)
        {
            if (id != command.Id)
                return BadRequest(); // Return 400 if IDs do not match

            var success = await _mediator.Send(command);

            if (!success)
                return NotFound(); // Return 404 if template not found

            return NoContent(); // Return 204 if update is successful
        }

        /// <summary>
        /// Soft delete a template
        /// </summary>
        /// <param name="id">template id</param>
        /// <returns>Message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var deleteCommand = new DeleteTemplateCommand { Id = id };
            var success = await _mediator.Send(deleteCommand);

            if (!success)
                return NotFound(); // Return 404 if template not found or already deleted

            return NoContent(); // Return 204 for successful soft delete
        }
    }
}