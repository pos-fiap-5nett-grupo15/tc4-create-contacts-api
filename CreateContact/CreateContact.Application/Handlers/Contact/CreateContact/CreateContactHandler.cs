using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Domain.Entities;
using CreateContact.Domain.Enums;
using CreateContact.Infrastructure.Services.Contact;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace CreateContact.Application.Handlers.Contact.CreateContact
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, CreateContactResponse>
    {
        private readonly IContactService _contactService;
        private readonly IValidator<CreateContactRequest> _validator;

        private const string EXCHANGE_NAME = "tech-challenge";
        private const string ROUTING_KEY = "tc3-create-contact-rk";
        private const string QUEUE_NAME = "tc3-create-contact-queue";

        public CreateContactHandler(
            IContactService contactService,
            IValidator<CreateContactRequest> validator)
        {
            _contactService = contactService;
            _validator = validator;
        }

        public async Task<CreateContactResponse> Handle(CreateContactRequest requisicao, CancellationToken ct)
        {
            if (Validate(requisicao) is var validacao && !string.IsNullOrWhiteSpace(validacao.ErrorDescription))
                return validacao;

            await _contactService.CreateAsync(Mapper(requisicao));

            await Publish(requisicao, ct);

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

        public static async Task Publish(CreateContactRequest request, CancellationToken ct)
        {
            // Criar uma conexão com o RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Converter a mensagem para bytes
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));

                // Enviar a mensagem para a fila
                await channel.BasicPublishAsync(EXCHANGE_NAME, ROUTING_KEY, body, ct);
            }
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
