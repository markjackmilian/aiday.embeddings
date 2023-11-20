using System.Text.Json.Serialization;

namespace aiday.embeddings.typesense.Models;

public class VectorsEntity
{
    [JsonPropertyName("vectors")]
    public float[] Vectors { get; set; }
}