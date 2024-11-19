using CreateContact.Api;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        var startup = new Startup(builder.Configuration);
        startup.ConfigureService(builder.Services);

        var app = builder.Build();

        startup.Configure(app, app.Environment);

        app.Run();
    }
}