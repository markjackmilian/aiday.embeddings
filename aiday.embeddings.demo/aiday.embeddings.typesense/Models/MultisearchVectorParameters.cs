using System.Globalization;
using System.Text.Json.Serialization;

namespace aiday.embeddings.typesense.Models
{
    public record MultiSearchVectorParameters
    {
        [JsonPropertyName("q")]
        public string Text { get; private set; }

        [JsonPropertyName("collection")]
        public string Collection { get; set; }

        [JsonPropertyName("vector_query")]
        public string VectorQuery { get; set; }

        public MultiSearchVectorParameters(string collection, string vectorQuery)
        {
            Text = "*";
            Collection = collection;
            VectorQuery = $"vectors:({vectorQuery}, k:5)";
        }
        
        public MultiSearchVectorParameters(string collection, float[] vectorQuery)
        {
            var query = $"[{string.Join(',', vectorQuery.Select(s=> s.ToString(CultureInfo.InvariantCulture)))}]";
            Text = "*";
            Collection = collection;
            VectorQuery = $"vectors:({query}, k:5)";
        }
    }
}
