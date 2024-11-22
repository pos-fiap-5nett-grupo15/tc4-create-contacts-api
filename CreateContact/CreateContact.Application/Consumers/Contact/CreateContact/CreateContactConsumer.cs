using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Worker.Messages;
using Microsoft.Extensions.Logging;
using TechChallenge.Common.RabbitMQ;
using TechChallenge.Domain.Enums;

namespace CreateContact.Application.Consumers.Contact.CreateContact
{
    public class CreateContactConsumer : ICreateContactConsumer
    {
        private readonly IContactService _contactService;
        private readonly ILogger<CreateContactConsumer> _logger;
        private readonly IRabbitMQProducerSettings _rabbitMQProducerSettings;

        public CreateContactConsumer(
            IContactService contactService,
            ILogger<CreateContactConsumer> logger,
            IRabbitMQProducerSettings rabbitMQProducerSettings)
        {
            _logger = logger;
            _contactService = contactService;
            _rabbitMQProducerSettings = rabbitMQProducerSettings;
        }

        public async Task HandleAsync(CreateContactMessage message, CancellationToken ct)
        {
            if (await _contactService.GetByIdAsync(message.Id) is var contact && contact is not null)
            {
                await _contactService.UpdateStatusByIdAsync(contact, ContactSituationEnum.CRIADO);
                return;
            }
            else
            {
                _logger.LogWarning($"An invalid message was received with contact id: {message.Id}.");

                await RabbitMQManager.Publish(
                    new CreateContactMessage { Id = message.Id },
                    _rabbitMQProducerSettings.Host,
                    _rabbitMQProducerSettings.Exchange,
                    _rabbitMQProducerSettings.RoutingKey,
                    ct);
                return;
            }
        }
    }
}