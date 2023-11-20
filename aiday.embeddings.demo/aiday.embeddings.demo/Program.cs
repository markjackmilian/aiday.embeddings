// See https://aka.ms/new-console-template for more information

using aiday.embeddings.typesense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();
var config = serviceProvider.GetService<IConfiguration>();
var test = config!.GetValue<int>("Test");
var tt = test;

static void ConfigureServices(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
            
    services.AddSingleton<IConfiguration>(configuration);

    var section = configuration.GetSection("Typesense");
    services.AddTypesense(config =>
    {
        config.Node.Port = section.GetValue<string>("Port");
        config.Node.Protocol = section.GetValue<string>("Protocol");
        config.ApiKey = section.GetValue<string>("ApiKey");
        config.Node.Host = section.GetValue<string>("Host");
    } );
}

