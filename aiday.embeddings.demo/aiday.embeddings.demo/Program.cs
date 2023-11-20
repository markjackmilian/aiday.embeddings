// See https://aka.ms/new-console-template for more information

using aiday.embeddings.demo;
using aiday.embeddings.demo.Models;
using aiday.embeddings.demo.Services;
using aiday.embeddings.typesense;
using aiday.embeddings.typesense.Models;
using Microsoft.Extensions.DependencyInjection;
using mjm.nethelpers;

var services = new ServiceCollection();
ServiceConfiguration.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();

var embeddingService = serviceProvider.GetService<IDemoEmbeddingService>();
var typeSenseClient = serviceProvider.GetService<ITypesenseClient>();

await RunnerHelper.RunAndManageException( () => typeSenseClient
    .CreateCollection(new Schema("aiday_demo", TypeSenseFeedback.GetSchema())), 
    exception =>
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    });

return;
var embedded = await embeddingService.CreateEmbeddings("Oggi ho voglia di mangiare una pizza");
var typeSenseFeedback = new TypeSenseFeedback
{
    Vectors = embedded.Embeddings,
    Id = Guid.NewGuid().ToString("N"),
    Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    Text = "Oggi ho voglia di mangiare una pizza",
};

var stored = await typeSenseClient.CreateDocument("aiday_demo", typeSenseFeedback);
var tt = stored;

// var test = config!.GetValue<int>("Test");
// var tt = test;



