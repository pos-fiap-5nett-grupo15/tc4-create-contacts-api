using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Worker.Messages;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using TechChallenge3.Common.RabbitMQ;
using TechChallenge3.Domain.Entities.Contact;
using TechChallenge3.Domain.Enums;

namespace CreateContact.Application.Handlers.Contact.CreateContact
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, CreateContactResponse>
    {
        private readonly IContactService _contactService;
        private readonly ILogger<CreateContactHandler> _logger;
        private readonly IValidator<CreateContactRequest> _validator;
        private readonly IRabbitMQProducerSettings _rabbitMQProducerSettings;

        public CreateContactHandler(
            IContactService contactService,
            ILogger<CreateContactHandler> logger,
            IValidator<CreateContactRequest> validator,
            IRabbitMQProducerSettings rabbitMQProducerSettings)
        {
            _logger = logger;
            _validator = validator;
            _contactService = contactService;
            _rabbitMQProducerSettings = rabbitMQProducerSettings;
        }

        public async Task<CreateContactResponse> Handle(CreateContactRequest requisicao, CancellationToken ct)
        {
            if (Validate(requisicao) is var validacao && !string.IsNullOrWhiteSpace(validacao.ErrorDescription))
                return validacao;

            //var test = await _contactService.GetByIdAsync(1);
            //var id = await _contactService.CreateAsync(Mapper(requisicao));

            await PublishByHostName(
                new CreateContactMessage { Id = 1 },
                _rabbitMQProducerSettings.Host,
                _rabbitMQProducerSettings.Port,
                _rabbitMQProducerSettings.Exchange,
                _rabbitMQProducerSettings.RoutingKey,
                ct);

            return new CreateContactResponse();
        }

        public CreateContactResponse Validate(CreateContactRequest requisicao)
        {
            var retorno = new CreateContactResponse();
            var result = _validator.Validate(requisicao);
            if (!result.IsValid)
            {
                var erroMensagem = "";
                foreach (var error in result.Errors)
                    erroMensagem += error.ErrorMessage + " ";

                retorno.ErrorDescription = erroMensagem;
            }

            return retorno;
        }

        public static ContactEntity Mapper(CreateContactRequest request) =>
            new(nome: request.Nome ?? string.Empty,
                email: request.Email ?? string.Empty,
                ddd: request.Ddd,
                telefone: request.Telefone,
                situacaoAnterior: null,
                situacaoAtual: ContactSituationEnum.PENDENTE_CRIACAO);

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

        public static async Task PublishByUri(
            object message,
            string uri,
            string exchangeName,
            string routingKeyName,
            CancellationToken ct)
        {
            // Criar uma conexão com o RabbitMQ
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(uri)
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
