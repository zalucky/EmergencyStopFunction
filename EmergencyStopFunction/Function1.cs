/*
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Devices;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace EmergencyStopFunction;


public class DeviceErrorProcessor
{
    private readonly ILogger _logger;
    private readonly ServiceClient _serviceClient;

    public DeviceErrorProcessor(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DeviceErrorProcessor>();
        var connStr = Environment.GetEnvironmentVariable("IOTHUB_CONNECTION");
        _serviceClient = ServiceClient.CreateFromConnectionString(connStr);
    }

    [Function("DeviceErrorProcessor")]
    public async Task RunAsync(
        [BlobTrigger("deviceerrorsoutputblob/{name}", Connection = "AzureWebJobsStorage")] string blobContent,
        string name)
    {
        _logger.LogInformation($"Blob triggered: {name}");

        var entries = new List<DeviceErrorEntry>();
        using (var reader = new StringReader(blobContent))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var entry = JsonSerializer.Deserialize<DeviceErrorEntry>(line);
                    if (entry != null)
                        entries.Add(entry);
                }
            }
        }

        if (entries is null) return;

        foreach (var entry in entries)
        {
            if (entry.ErrorCount > 3)
            {
                string iotHubDeviceId = entry.DeviceName.Replace(" ", "-").ToLower();
                _logger.LogWarning($"Triggering EmergencyStop on {entry.DeviceName} (mapped to: {iotHubDeviceId})");

                var method = new CloudToDeviceMethod("EmergencyStop");

                try
                {
                    var result = await _serviceClient.InvokeDeviceMethodAsync(iotHubDeviceId, method);
                    _logger.LogInformation($"Device: {iotHubDeviceId}, Status: {result.Status}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"InvokeDeviceMethodAsync failed for device '{iotHubDeviceId}': {ex.Message}");
                }
            }
        }
    }
}


public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (DateTime.TryParse(value, out var result))
            return result;

        throw new JsonException($"Cannot parse DateTime: {value}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("o")); // ISO 8601
    }
}


public class DeviceErrorEntry
{
    public string DeviceName { get; set; }
    public int ErrorCount { get; set; }
    //public DateTime DetectedAt { get; set; }
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime DetectedAt { get; set; }
}

*/