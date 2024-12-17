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

                    await PublishByHostName(
                        new CreateContactMessage { Id = 1 },
                        _rabbitMQProducerSettings.Host,
                        _rabbitMQProducerSettings.Port,
                        _rabbitMQProducerSettings.Exchange,
                        _rabbitMQProducerSettings.RoutingKey,
                        ct);
                    return;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }


        public static async Task PublishByHostName(
            object message,
            string hostName,
            int port,
            string exchangeName,
            string routingKeyName,
            CancellationToken ct)
        {
            // Criar uma conexão com o RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = "guest",
                Password = "guest",
            };
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Converter a mensagem para bytes
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // Enviar a mensagem para a fila
                await channel.BasicPublishAsync(exchangeName, routingKeyName, body, ct);
            }
        }
    }
}