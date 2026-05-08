using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirlineBookingApi.Configurations;

/// <summary>
/// Ensures all DateTime values are serialized with UTC "Z" suffix so JavaScript clients
/// can parse them unambiguously as UTC (preventing timezone-offset display errors).
/// </summary>
public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dt = reader.GetDateTime();
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(DateTime.SpecifyKind(value, DateTimeKind.Utc).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
    }
}
