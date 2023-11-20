using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using aiday.embeddings.typesense.Exceptions;
using aiday.embeddings.typesense.Models;
using Microsoft.Extensions.Options;

namespace aiday.embeddings.typesense
{
    public interface ITypesenseClient
    {
        Task<CollectionResponse> CreateCollection(Schema schema);
        Task<CollectionResponse> DeleteCollection(string name);
        Task<bool> DeleteDocument(string collectionName, string documentId);
        Task<T> CreateDocument<T>(string collection, T document) where T : class;
        Task<SearchResult<T>> VectorSearch<T>(MultiSearchVectorParameters parameters);
    }

    public class TypesenseClient : ITypesenseClient
    {
        private readonly HttpClient _client;

        public TypesenseClient(IOptions<TypeSenseConfig> config, HttpClient client)
        {
            this._client = client;
            client.BaseAddress = new Uri($"{config.Value.Node.Protocol}://{config.Value.Node.Host}:{config.Value.Node.Port}");
            client.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", config.Value.ApiKey);
        }

        private readonly JsonSerializerOptions _jsonNameCaseInsentiveTrue = new() { PropertyNameCaseInsensitive = true };
        private readonly JsonSerializerOptions _jsonOptionsCamelCaseIgnoreWritingNull = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<CollectionResponse> CreateCollection(Schema schema)
        {
            if (schema is null)
                throw new ArgumentNullException(nameof(schema));

            var json = JsonSerializer.Serialize(schema, _jsonOptionsCamelCaseIgnoreWritingNull);
            var response = await this._client.PostAsync("/collections", GetApplicationJsonStringContent(json));

            var responseString = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
            ? CheckResponse<CollectionResponse>(responseString, _jsonNameCaseInsentiveTrue)
            : throw GetException(response.StatusCode, responseString);
        }

        public async Task<CollectionResponse> DeleteCollection(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Cannot be null empty or whitespace", nameof(name));
            
            var response = await this._client.DeleteAsync($"/collections/{name}");
            var responseString = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? CheckResponse<CollectionResponse>(responseString, _jsonNameCaseInsentiveTrue)
                : throw GetException(response.StatusCode, responseString);
            
        }

        public async Task<bool> DeleteDocument(string collectionName, string documentId)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null empty or whitespace", nameof(collectionName));
            if (documentId is null)
                throw new ArgumentNullException(nameof(documentId));
            
            var response = await this._client.DeleteAsync($"/collections/{collectionName}/documents/{documentId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<T> CreateDocument<T>(string collection, T document) where T : class
        {
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentException("Cannot be null empty or whitespace", nameof(collection));
            if (document is null)
                throw new ArgumentNullException(nameof(document));

            var json = JsonSerializer.Serialize(document, _jsonOptionsCamelCaseIgnoreWritingNull); 
            var response = await this._client.PostAsync($"/collections/{collection}/documents", GetApplicationJsonStringContent(json));
            var responseString = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode
                ? CheckResponse<T>(responseString, _jsonNameCaseInsentiveTrue)
                : throw GetException(response.StatusCode, responseString);
        }

        public async Task<SearchResult<T>> VectorSearch<T>(MultiSearchVectorParameters parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            var searches = new { Searches = new MultiSearchVectorParameters[] { parameters } };
            var json = JsonSerializer.Serialize(searches, _jsonOptionsCamelCaseIgnoreWritingNull);

            var response = await this._client.PostAsync($"/multi_search", GetApplicationJsonStringContent(json));
            var responseString = await response.Content.ReadAsStringAsync();
            
            var result = JsonSerializer.Deserialize<JsonElement>(responseString).TryGetProperty("results", out var results);
            if (!result)
                throw new InvalidOperationException($"Could not deserialize {typeof(T)}, Received following from Typesense: '{results[0]}'");

            responseString = JsonSerializer.Serialize(results[0]);

            return response.IsSuccessStatusCode
                ? CheckResponse<SearchResult<T>>(responseString, _jsonNameCaseInsentiveTrue)
                : throw GetException(response.StatusCode, responseString);
        }

        private T CheckResponse<T>(string json, JsonSerializerOptions options) where T : class
        {
            return !string.IsNullOrEmpty(json)
                ? JsonSerializer.Deserialize<T>(json, options) ?? throw new ArgumentException("Deserialize is not allowed to return null.")
                : throw new ArgumentException("Empty JSON response is not valid.");
        }

        private static StringContent GetApplicationJsonStringContent(string jsonString)
            => new(jsonString, Encoding.UTF8, "application/json");

        private static TypesenseApiException GetException(HttpStatusCode statusCode, string message)
            => statusCode switch
            {
                HttpStatusCode.BadRequest => new TypesenseApiBadRequestException(message),
                HttpStatusCode.Unauthorized => new TypesenseApiUnauthorizedException(message),
                HttpStatusCode.NotFound => new TypesenseApiNotFoundException(message),
                HttpStatusCode.Conflict => new TypesenseApiConflictException(message),
                HttpStatusCode.UnprocessableEntity => new TypesenseApiUnprocessableEntityException(message),
                HttpStatusCode.ServiceUnavailable => new TypesenseApiUnprocessableEntityException(message),
                _ => throw new ArgumentException($"Could not convert statuscode {Enum.GetName(statusCode)}.")
            };
    }
}
