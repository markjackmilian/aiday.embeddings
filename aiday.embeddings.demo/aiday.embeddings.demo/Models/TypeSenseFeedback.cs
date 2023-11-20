using System.Text.Json.Serialization;
using aiday.embeddings.typesense.Models;

namespace aiday.embeddings.demo.Models;

class VectorsEntity
{
    [JsonPropertyName("vectors")] public float[] Vectors { get; set; }
}

class TypeSenseFeedback : VectorsEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("date")]
    public long Date { get; set; }
    
    [JsonPropertyName("text")]
    public string Text { get; set; }

    public static List<Field> GetSchema() => new()
    {
        new("vectors", FieldType.FloatArray, false, true, 1536),
        new("text", FieldType.String),
        new("date", FieldType.Int64),
        new("id", FieldType.String),
    };
}