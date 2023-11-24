using System.Globalization;
using CsvHelper;

namespace aiday.embeddings.demo.Services;

record Faq(string Question, string Url);

interface ICsvService
{
    Faq[] ReadCsv(string path);
}

class CsvService : ICsvService
{
    public Faq[] ReadCsv(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<Faq>();
        return records.ToArray();
    }
}