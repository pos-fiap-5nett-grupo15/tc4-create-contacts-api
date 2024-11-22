using MediatR;

namespace CreateContact.Worker.Messages
{
    public class CreateContactMessage
    {
        public int Id { get; set; }
    }
}
