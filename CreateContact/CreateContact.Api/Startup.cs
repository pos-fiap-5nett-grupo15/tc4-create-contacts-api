using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Application.DTOs.Validations;
using CreateContact.Application.Handlers.Contact.CreateContact;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.Settings;
using CreateContact.Infrastructure.UnitOfWork;
using FluentValidation;
using Serilog;
using TechChallenge3.Infrastructure.DefaultStartup;

namespace CreateContact.Api
{
    internal class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
            this.Configuration = configuration;
        }

        internal void ConfigureImpl(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            this.Configure(app, environment);
        }

        internal void ConfigureServiceImpl(WebApplicationBuilder builder)
        {
            this.ConfigureService(builder.Services);
            builder.Host.UseSerilog();
            builder.Services.AddLogging();

            ConfigureUnitOfWork(builder.Services);
            ConfigureHandleServices(builder.Services);
            ConfigureContactServices(builder.Services);
        }

        private void ConfigureContactServices(IServiceCollection services)
        {
            services.AddScoped<IContactService, ContactService>();
        }

        private void ConfigureHandleServices(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQProducerSettings>(Configuration.GetSection(nameof(RabbitMQProducerSettings))?.Get<RabbitMQProducerSettings>() ?? throw new ArgumentNullException(nameof(RabbitMQProducerSettings)));

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateContactHandler).Assembly));
            services.AddTransient<IValidator<CreateContactRequest>, ContactValidation>();
        }

        private void ConfigureUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<ICreateContactUnitOfWork, CreateContactUnitOfWork>();
        }
    }
}
