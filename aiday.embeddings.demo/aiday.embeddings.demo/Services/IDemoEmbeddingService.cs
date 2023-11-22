using OpenAI.Net;
using OpenAI.Net.Models.Requests;
using Throw;

namespace aiday.embeddings.demo.Services;

record EmbeddingResponse(float[] Embeddings, int Tokens);

interface IDemoEmbeddingService
{
    Task<EmbeddingResponse> CreateEmbeddings(string text);
}

class DemoEmbeddingService(IOpenAIService openAiService) : IDemoEmbeddingService
{
    public async Task<EmbeddingResponse> CreateEmbeddings(string text)
    {
        var response =
            await openAiService.Embeddings.Create(new EmbeddingsRequest(text,
                "text-embedding-ada-002"));

        response.IsSuccess.Throw(() => throw response.Exception).IfFalse();
        var embeddings = response.Result!.Data[0].Embedding;
        return new EmbeddingResponse(embeddings.Select(s=> (float)s).ToArray(),response.Result.Usage.Total_tokens);
    }
}