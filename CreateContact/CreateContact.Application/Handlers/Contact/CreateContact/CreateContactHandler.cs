using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Worker.Messages;
using FluentValidation;
using MediatR;
using TechChallenge3.Common.LogSettings;
using TechChallenge3.Common.RabbitMQ;
using TechChallenge3.Domain.Entities.Contact;
using TechChallenge3.Domain.Enums;

namespace CreateContact.Application.Handlers.Contact.CreateContact
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, CreateContactResponse>
    {
        private readonly IContactService _contactService;
        private readonly IGraylogger _logger;
        private readonly IValidator<CreateContactRequest> _validator;
        private readonly IRabbitMQProducerSettings _rabbitMQProducerSettings;

        public CreateContactHandler(
            IContactService contactService,
            IGraylogger logger,
            IValidator<CreateContactRequest> validator,
            IRabbitMQProducerSettings rabbitMQProducerSettings)
        {
            _logger = logger;
            _validator = validator;
            _contactService = contactService;
            _rabbitMQProducerSettings = rabbitMQProducerSettings;
        }

        public async Task<CreateContactResponse> Handle(CreateContactRequest request, CancellationToken ct)
        {
            try
            {
                if (await Validate(request) is var validationResult && !string.IsNullOrWhiteSpace(validationResult.ErrorDescription))
                    return validationResult;

                await _logger.LogInformation($"Starting to create the '{nameof(CreateContactRequest)}' as '{ContactSituationEnum.PENDENTE_CRIACAO}'.");

                var id = await _contactService.CreateAsync(Mapper(request));

                await _logger.LogInformation($"New contact created with ID '{id}'");

                await _logger.LogInformation($"Starting to publish the '{nameof(CreateContactMessage)}' in RabbitMQ.");

                await RabbitMQManager.PublishAsync(
                    message: new CreateContactMessage { Id = id },
                    hostName: _rabbitMQProducerSettings.Host,
                    port: _rabbitMQProducerSettings.Port,
                    userName: _rabbitMQProducerSettings.Username,
                    password: _rabbitMQProducerSettings.Password,
                    exchangeName: _rabbitMQProducerSettings.Exchange,
                    routingKeyName: _rabbitMQProducerSettings.RoutingKey,
                    ct);

                await _logger.LogInformation($"'{nameof(CreateContactMessage)}' published in RabbitMQ.");

                return new CreateContactResponse();
            }
            catch (Exception e)
            {
                await _logger.LogError(e, "An error occurr while creating contact.");

                throw;
            }
        }

        public async Task<CreateContactResponse> Validate(CreateContactRequest requisicao)
        {
            await _logger.LogInformation($"Starting to validate the '{nameof(CreateContactRequest)}'.");
            var retorno = new CreateContactResponse();
            var result = _validator.Validate(requisicao);
            if (!result.IsValid)
            {
                var erroMensagem = "";
                foreach (var error in result.Errors)
                    erroMensagem += error.ErrorMessage + " ";

                retorno.ErrorDescription = erroMensagem;
            }
            await _logger.LogInformation($"Finalizing request validation.");

            return retorno;
        }

        public static ContactEntity Mapper(CreateContactRequest request) =>
            new(nome: request.Nome ?? string.Empty,
                email: request.Email ?? string.Empty,
                ddd: request.Ddd,
                telefone: request.Telefone,
                situacaoAnterior: null,
                situacaoAtual: ContactSituationEnum.PENDENTE_CRIACAO);
    }
}
