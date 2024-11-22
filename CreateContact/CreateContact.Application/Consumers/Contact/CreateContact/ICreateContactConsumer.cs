using CreateContact.Worker.Messages;

namespace CreateContact.Application.Consumers.Contact.CreateContact
{
    public interface ICreateContactConsumer
    {
        Task HandleAsync(CreateContactMessage message, CancellationToken ct);
    }
}
