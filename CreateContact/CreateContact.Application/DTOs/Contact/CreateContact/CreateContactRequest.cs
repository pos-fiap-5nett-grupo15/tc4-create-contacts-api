using MediatR;

namespace CreateContact.Application.DTOs.Contact.CreateContact
{
    public class CreateContactRequest : IRequest<CreateContactResponse>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public int Ddd { get; set; }
        public int Telefone { get; set; }
    }
}
