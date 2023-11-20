using aiday.embeddings.demo.Services;
using aiday.embeddings.typesense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Net;

namespace aiday.embeddings.demo;

static class ServiceConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<Program>()
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
    
        services.AddOpenAIServices(options => { options.ApiKey = configuration["OpenAiKey"]!; });
        services.AddSingleton<IDemoEmbeddingService, DemoEmbeddingService>();
    }
}