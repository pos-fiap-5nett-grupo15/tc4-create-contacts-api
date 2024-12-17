using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Worker.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using TechChallenge3.Common.RabbitMQ;
using TechChallenge3.Domain.Enums;

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
            try
            {
                if (await _contactService.GetByIdAsync(message.Id) is var contact && contact is not null)
                {
                    await _contactService.UpdateStatusByIdAsync(contact, ContactSituationEnum.CRIADO);
                    return;
                }
                else
                {
                    _logger.LogWarning($"An invalid message was received with contact id: {message.Id}.");

                    await RabbitMQManager.PublishAsync(
                        message: new CreateContactMessage { Id = message.Id },
                        hostName: _rabbitMQProducerSettings.Host,
                        port: _rabbitMQProducerSettings.Port,
                        userName: _rabbitMQProducerSettings.Username,
                        password: _rabbitMQProducerSettings.Password,
                        exchangeName: _rabbitMQProducerSettings.Exchange,
                        routingKeyName: _rabbitMQProducerSettings.RoutingKey,
                        ct);
                    return;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurr while consume contact ID '{message.Id}': {e.Message}.");

                await RabbitMQManager.PublishAsync(
                    message: new CreateContactMessage { Id = message.Id },
                    hostName: _rabbitMQProducerSettings.Host,
                    port: _rabbitMQProducerSettings.Port,
                    userName: _rabbitMQProducerSettings.Username,
                    password: _rabbitMQProducerSettings.Password,
                    exchangeName: _rabbitMQProducerSettings.Exchange,
                    routingKeyName: _rabbitMQProducerSettings.RoutingKey,
                    ct);

                throw;
            }

        }
    }
}