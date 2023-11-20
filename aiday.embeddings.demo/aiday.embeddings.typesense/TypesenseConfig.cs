using Microsoft.Extensions.DependencyInjection;

namespace aiday.embeddings.typesense
{
    public static class TypesenseConfig
    {
        public static IServiceCollection AddTypesense(this IServiceCollection services, Action<TypeSenseConfig> configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration), "Please provide a configuration");

            return services
                .AddScoped<ITypesenseClient, TypesenseClient>()
                .AddHttpClient<ITypesenseClient, TypesenseClient>().Services
                .Configure(configuration);
        }
    }

    public record TypeSenseConfig
    {
        public Node Node { get; set; }
        public string ApiKey { get; set; }

        public TypeSenseConfig()
        {
            Node = new Node();
        }
    }

    public record Node
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }

    }
}
