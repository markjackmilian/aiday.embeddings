using System.Text.Json.Serialization;

namespace aiday.embeddings.typesense.Models
{
    public record SearchResult<T>
    {
        [JsonPropertyName("hits")]
        public IReadOnlyList<Hit<T>> Hits { get; init; }

        [JsonPropertyName("found")]
        public int Found { get; init; }

        [JsonConstructor]
        public SearchResult(IReadOnlyList<Hit<T>> hits)
        {
            Hits = hits;
        }
    }

    public record Hit<T>
    {
        [JsonPropertyName("document")]
        public T Document { get; init; }

        [JsonPropertyName("text_match")]
        public long TextMatch { get; init; }
        
        [JsonPropertyName("vector_distance")]
        public double VectorDistance { get; set; }
    }
}
