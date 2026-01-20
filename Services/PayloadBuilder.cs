using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SignHmacTutorial.Services;

public static class PayloadBuilder
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Indented = false
    };

    // Strict schema from the original console app:
    // {"sapSoldToAccount":"...","emsId":199972,"emFirmId":"...","productDomain":"..."}
    public static string BuildAitJson(string sapSoldToAccount, long emsId, string emFirmId, string productDomain)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, WriterOptions))
        {
            writer.WriteStartObject();
            writer.WriteString("sapSoldToAccount", sapSoldToAccount);
            writer.WriteNumber("emsId", emsId);
            writer.WriteString("emFirmId", emFirmId);
            writer.WriteString("productDomain", productDomain);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    // Strict schema from the original console app:
    // {"EmsId":"199972","SapSoldToAccount":"...","EmFirmId":"..."}
    public static string BuildAiaJson(string emsId, string sapSoldToAccount, string emFirmId)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, WriterOptions))
        {
            writer.WriteStartObject();
            writer.WriteString("EmsId", emsId);
            writer.WriteString("SapSoldToAccount", sapSoldToAccount);
            writer.WriteString("EmFirmId", emFirmId);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
