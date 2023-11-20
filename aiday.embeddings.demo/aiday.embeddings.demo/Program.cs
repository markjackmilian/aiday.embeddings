// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
ConfigureServices(services);


static void ConfigureServices(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
            
    services.AddSingleton<IConfiguration>(configuration);
}

