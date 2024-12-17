using CreateContact.Application.DTOs.Contact.CreateContact;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechChallenge3.Common.DTOs;

namespace CreateContact.Api.Controllers.Contacts
{
    [ApiController]
    [Route("[controller]")]
    // TODO: Implementar autenticação/autorização
    public class ContactsController
        : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactsController(IMediator mediator) =>
            _mediator = mediator;

        [HttpPost("create")]
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
                throw e;
            }
        }
    }
}
