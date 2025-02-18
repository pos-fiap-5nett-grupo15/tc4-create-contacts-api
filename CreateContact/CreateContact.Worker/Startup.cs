using CreateContact.Application.Consumers.Contact.CreateContact;
using CreateContact.Infrastructure.RabbitMQ;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Infrastructure.UnitOfWork;
using CreateContact.Worker.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using TechChallenge3.Infrastructure.Crypto;
using TechChallenge3.Infrastructure.Settings;
using TechChallenge3.Infrastructure.UnitOfWork;

namespace CreateContact.Worker
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

            this.ConfigureRabitMQ(services);
            this.ConfigureDatabase(services);
            this.ConfigureCustomServices(services);
            this.ConfigureConsumers(services);
        }

        private void ConfigureRabitMQ(IServiceCollection services)
        {
            var rabbitMQProducerSettings = Configuration.GetSection(nameof(RabbitMQProducerSettings))?.Get<RabbitMQProducerSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQProducerSettings));
            var rabbitMQConsumerSettings = Configuration.GetSection(nameof(RabbitMQConsumerSettings))?.Get<RabbitMQConsumerSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQConsumerSettings));

            System.Console.WriteLine(JsonConvert.SerializeObject($"RabbitMQProducerSettings: {rabbitMQProducerSettings}"));
            System.Console.WriteLine(JsonConvert.SerializeObject($"RabbitMQConsumerSettings: {rabbitMQConsumerSettings}"));

            services.AddSingleton<IRabbitMQProducerSettings>(rabbitMQProducerSettings);
            services.AddSingleton(new RabbitMQConnector(rabbitMQConsumerSettings));
            services.AddHostedService<RabbitMQConsumer>();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var cryptoService = new CryptoService(Configuration.GetSection(nameof(CryptoSettings)).Get<CryptoSettings>());
            services.AddSingleton((ICryptoService)cryptoService);
            services.AddScoped<ITechDatabase, TechDatabase>();
            services.AddScoped<ICreateContactUnitOfWork, CreateContactUnitOfWork>();
        }

        private void ConfigureCustomServices(IServiceCollection services) =>
            services.AddScoped<IContactService, ContactService>();

        private void ConfigureConsumers(IServiceCollection services) =>
            services.AddScoped<ICreateContactConsumer, CreateContactConsumer>();

    }
}
