using System.Text.Json.Serialization;

namespace aiday.embeddings.typesense.Models
{
    public record Schema
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("fields")]
        public IEnumerable<Field> Fields { get; init; }

        public Schema(string name, IEnumerable<Field> fields)
        {
            Name = name;
            Fields = fields;
        }
    }
}
