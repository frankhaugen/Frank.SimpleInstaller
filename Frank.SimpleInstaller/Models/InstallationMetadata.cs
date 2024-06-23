using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frank.SimpleInstaller.Models;

public class InstallationMetadata
{
    /// <summary>
    /// The version of the installation.
    /// </summary>
    public Version Version { get; set; }

    /// <summary>
    /// The name of the installation.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The executable name.
    /// </summary>
    public string ExecutableName { get; set; }

    /// <summary>
    /// Returns a JSON representation of the metadata.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }

    public static InstallationMetadata? Load(Stream metadataStream)
    {
        return JsonSerializer.Deserialize<InstallationMetadata>(metadataStream, JsonSerializerOptions);
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
}