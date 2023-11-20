using System.Text.Json.Serialization;
using aiday.embeddings.typesense.Converters;

namespace aiday.embeddings.typesense.Models
{
    public record Field
    {
        [JsonPropertyName("name")]
        public string Name { get; init; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(Converters.JsonStringEnumConverter<FieldType>))]
        public FieldType Type { get; init; }

        [JsonPropertyName("optional")]
        public bool? Optional { get; init; }

        [JsonPropertyName("index")]
        public bool? Index { get; init; }

        [JsonPropertyName("num_dim")]
        public int? NumDim { get; init; }

        public Field(string name, FieldType type)
        {
            Name = name;
            Type = type;
        }

        public Field()
        {
            
        }

        public Field(string name, FieldType type, bool optional, bool index, int numDim)
        {
            Name = name;
            Type= type;
            Optional = optional;
            Index = index;
            NumDim = numDim;
        }
    }
}