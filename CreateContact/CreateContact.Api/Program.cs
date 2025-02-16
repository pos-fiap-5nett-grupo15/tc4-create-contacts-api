using CreateContact.Api;


internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup(builder.Configuration);
        startup.ConfigureServiceImpl(builder);

        var app = builder.Build();
        startup.ConfigureImpl(app, app.Environment);
        app.Run();
    }
}