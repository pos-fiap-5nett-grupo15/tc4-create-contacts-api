using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Application.DTOs.Validations;
using CreateContact.Domain.Entities;
using CreateContact.Infrastructure.Services.Contact;
using MediatR;

namespace CreateContact.Application.Handlers.Contact.CreateContact
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, CreateContactResponse>
    {
        private readonly IContactService _contactService;

        public CreateContactHandler(IContactService contactService) =>
            _contactService = contactService;

        public async Task<CreateContactResponse> Handle(CreateContactRequest request, CancellationToken cancellationToken)
        {
            var validator = new ContactValidation();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var erroMensagem = "";

                foreach (var error in result.Errors)
                {
                    erroMensagem += error.ErrorMessage + " ";
                }

                var retorno = new CreateContactResponse();
                retorno.ErrorDescription = erroMensagem;

                return retorno;

            }
            else
            {
                await _contactService.CreateAsync(Mapper(request));
                return new CreateContactResponse();
            }
        }


        public static ContactEntity Mapper(CreateContactRequest request) =>
            new(nome: request.Nome ?? string.Empty,
                email: request.Email ?? string.Empty,
                ddd: request.Ddd,
                telefone: request.Telefone);

    }

}
