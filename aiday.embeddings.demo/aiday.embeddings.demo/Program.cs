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
var csvService = serviceProvider.GetService<ICsvService>();

var faqs = csvService.ReadCsv("openai_faq.csv");

await RunnerHelper.RunAndManageException( () => typeSenseClient
    .CreateCollection(new Schema("aiday_demo", TypeSenseFeedback.GetSchema())), 
    exception =>
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    });

var totalTokenCost = 0;
foreach (var faq in faqs)
{
    Console.WriteLine($"Creating embedding for: {faq.Question}");
    var embedded = await embeddingService.CreateEmbeddings(faq.Question);
    var typeSenseFeedback = new TypeSenseFeedback
    {
        Vectors = embedded.Embeddings,
        Id = Guid.NewGuid().ToString("N"),
        Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        Text = faq.Question,
        Url = faq.Url
    };
    Console.WriteLine($"Created embedding for: {faq.Question} - Token: {embedded.Tokens}");
    totalTokenCost+= embedded.Tokens;
    await typeSenseClient.CreateDocument("aiday_demo", typeSenseFeedback);
    Console.WriteLine("Saved to typesense");
}

Console.WriteLine($"Total token cost: {totalTokenCost} ");






