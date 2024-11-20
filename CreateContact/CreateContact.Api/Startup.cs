using CreateContact.Application.DTOs.Contact.CreateContact;
using CreateContact.Application.DTOs.Validations;
using CreateContact.Application.Handlers.Contact.CreateContact;
using CreateContact.Infrastructure.Services.Contact;
using CreateContact.Infrastructure.UnitOfWork;
using FluentValidation;
using TechChallenge.Infrastructure.DefaultStartup;

namespace CreateContact.Api
{
    internal class Startup : BaseStartup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
            this.Configuration = configuration;
        }

        internal void ConfigureImpl(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            this.Configure(app, environment);
        }

        internal void ConfigureServiceImpl(IServiceCollection services)
        {
            this.ConfigureService(services);

            ConfigureUnitOfWork(services, this.Configuration);
            ConfigureHandleServices(services);
            ConfigureContactServices(services);
        }

        private void ConfigureContactServices(IServiceCollection services)
        {
            services.AddScoped<IContactService, ContactService>();
        }

        private void ConfigureHandleServices(IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateContactHandler).Assembly));
            services.AddTransient<IValidator<CreateContactRequest>, ContactValidation>();
        }

        private void ConfigureUnitOfWork(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICreateContactUnitOfWork, CreateContactUnitOfWork>();
        }
    }
}
