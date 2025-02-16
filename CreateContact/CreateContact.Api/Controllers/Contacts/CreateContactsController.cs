using CreateContact.Application.DTOs.Contact.CreateContact;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechChallenge3.Common.DTOs;
using TechChallenge3.Common.LogSettings;

namespace CreateContact.Api.Controllers.Contacts
{
    [ApiController]
    [Route("[controller]")]
    // TODO: Implementar autenticação/autorização no futuro
    public class CreateContactsController
        : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGraylogger _logger;

        public CreateContactsController(
            IMediator mediator,
            IGraylogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(BaseReponse))]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateContactRequest request)
        {
            try
            {
                return Created(string.Empty, await _mediator.Send(request));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
