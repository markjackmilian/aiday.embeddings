using aiday.embeddings.demo.Models;
using aiday.embeddings.demo.Services;
using aiday.embeddings.typesense;
using aiday.embeddings.typesense.Models;
using Microsoft.Extensions.DependencyInjection;
using mjm.nethelpers;

namespace aiday.embeddings.demo;

static class App
{
    private const string CollectionName = "aiday_demo_2";

    public static async Task CreateEmbeddings(ServiceProvider serviceProvider)
    {
        var embeddingService = serviceProvider.GetService<IDemoEmbeddingService>();
        var typeSenseClient = serviceProvider.GetService<ITypesenseClient>();
        var csvService = serviceProvider.GetService<ICsvService>();

        var faqs = csvService.ReadCsv("openai_faq.csv");

        await typeSenseClient
            .CreateCollection(new Schema(CollectionName, TypeSenseFeedback.GetSchema()));

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
            totalTokenCost += embedded.Tokens;
            await typeSenseClient.CreateDocument(CollectionName, typeSenseFeedback);
            Console.WriteLine("Saved to typesense");
        }

        Console.WriteLine($"Total token cost: {totalTokenCost} ");
    }

    public static async Task QueryEmbeddings(ServiceProvider serviceProvider)
    {
        var typeSenseClient = serviceProvider.GetService<ITypesenseClient>();
        var embeddingService = serviceProvider.GetService<IDemoEmbeddingService>();

        Console.WriteLine("Enter query:");
        var query = Console.ReadLine();
        
        Console.WriteLine("Creating embedding");
        var embedded = await embeddingService.CreateEmbeddings(query);
        
        var searchResult = await typeSenseClient.VectorSearch<TypeSenseFeedback>( new MultiSearchVectorParameters(CollectionName, embedded.Embeddings));
        
        foreach (var searchResultHit in searchResult.Hits.OrderBy(o=>o.VectorDistance).Take(3))
        {
            Console.WriteLine($"Distance: {searchResultHit.VectorDistance} - {searchResultHit.Document.Text}");
        }
        
    }
    
}