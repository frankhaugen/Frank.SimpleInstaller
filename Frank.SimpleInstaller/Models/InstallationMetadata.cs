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
    /// If the name contains characters that are not allowed in a folder name, this property contains a safe version of the name.
    /// </summary>
    public string? SafeName { get; set; }

    /// <summary>
    /// The company that owns the installation. If not specified, the installation will be placed in the root of the start menu.
    /// </summary>
    public string? Company { get; set; }

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