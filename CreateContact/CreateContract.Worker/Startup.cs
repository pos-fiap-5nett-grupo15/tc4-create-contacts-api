using CreateContact.Application.Consumers.Contact.CreateContact;
using CreateContact.Infrastructure.RabbitMQ;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Infrastructure.UnitOfWork;
using CreateContract.Worker.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using TechChallenge.Infrastructure.Crypto;
using TechChallenge.Infrastructure.Settings;
using TechChallenge.Infrastructure.UnitOfWork;

namespace CreateContract.Worker
{
    internal class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration) =>
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        internal void Configure(IApplicationBuilder app, IWebHostEnvironment environment) { }

        internal void ConfigureService(IServiceCollection services)
        {
            // Logging
            services.AddLogging();

            // Add services to the container.
            services.AddSingleton<IRabbitMQProducerSettings>(Configuration.GetSection(nameof(RabbitMQProducerSettings))?.Get<RabbitMQProducerSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQProducerSettings)));
            services.AddSingleton(new RabbitMQConnector(Configuration.GetSection(nameof(RabbitMQConsumerSettings))?.Get<RabbitMQConsumerSettings>()));
            services.AddHostedService<RabbitMQConsumer>();

            // Database
            var cryptoService = new CryptoService(Configuration.GetSection(nameof(CryptoSettings)).Get<CryptoSettings>());
            services.AddSingleton((ICryptoService)cryptoService);
            services.AddScoped<ITechDatabase, TechDatabase>();
            services.AddScoped<ICreateContactUnitOfWork, CreateContactUnitOfWork>();

            services.AddScoped<IContactService, ContactService>();

            services.AddScoped<ICreateContactConsumer, CreateContactConsumer>();
        }
    }
}
