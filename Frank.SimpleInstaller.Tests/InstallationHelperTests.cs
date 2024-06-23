using System.IO.Compression;

using Frank.SimpleInstaller.Helpers;
using Frank.SimpleInstaller.Models;

namespace Frank.SimpleInstaller.Tests;

public class InstallationHelperTests
{
    [Fact]
    public void Uninstall_WithValidMetadata_DeletesInstallationAndStartMenuDirectories()
    {
        // Arrange
        var installerFile = new FileInfo("Test1.zip");
        if (installerFile.Exists) installerFile.Delete(); // Ensure the file does not exist before creating it
        
        using var zip = ZipFile.Open(installerFile.FullName, ZipArchiveMode.Create);
        var metadata = new InstallationMetadata
        {
            Name = "Test1",
            Version = new Version(1, 0, 0),
            ExecutableName = "Test1.exe"
        };

        var metadataEntry = zip.CreateEntry(Constants.MetadataFilename);
        using var metadataStream = metadataEntry.Open();
        var metadataJson = metadata.ToString();
        using var writer1 = new StreamWriter(metadataStream);
        writer1.Write(metadataJson);
        writer1.Close();
        metadataStream.Close();

        var sourceFile = zip.CreateEntry($"{Constants.SourceFolderName}/Test1.exe");
        using var sourceFileStream = sourceFile.Open();
        using var writer2 = new StreamWriter(sourceFileStream);
        var buffer = new byte[1024];
        Random.Shared.NextBytes(buffer);
        writer2.Write(buffer);
        writer2.Close();
        sourceFileStream.Close();
        // ReSharper disable once DisposeOnUsingVariable
        zip.Dispose();
        var installResult = InstallationHelper.Install(installerFile);
        Assert.True(installResult);

        // Act
        var result = InstallationHelper.Uninstall(metadata);

        // Assert
        Assert.True(result);
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(metadata, false);
        Assert.False(installationDirectory.Exists);
        var startMenuDirectory = OperatingSystemHelper.GetStartMenuApplicationDirectory(metadata, false);
        Assert.False(startMenuDirectory.Exists);
    }

    [Fact]
    public void Install_WithValidInstallerFile_InstallsFiles()
    {
        // Arrange
        var installerFile = new FileInfo("Test.zip");
        if (installerFile.Exists) installerFile.Delete(); // Ensure the file does not exist before creating it
        
        using var zip = ZipFile.Open(installerFile.FullName, ZipArchiveMode.Create);
        var metadata = new InstallationMetadata
        {
            Name = "Test3",
            Version = new Version(1, 0, 0),
            ExecutableName = "Test3.exe"
        };

        var metadataEntry = zip.CreateEntry(Constants.MetadataFilename);
        using var metadataStream = metadataEntry.Open();
        var metadataJson = metadata.ToString();
        using var writer1 = new StreamWriter(metadataStream);
        writer1.Write(metadataJson);
        writer1.Close();
        metadataStream.Close();

        var sourceFile = zip.CreateEntry($"{Constants.SourceFolderName}/Test.exe");
        using var sourceFileStream = sourceFile.Open();
        using var writer2 = new StreamWriter(sourceFileStream);
        var buffer = new byte[1024];
        Random.Shared.NextBytes(buffer);
        writer2.Write(buffer);
        writer2.Close();
        sourceFileStream.Close();
        // ReSharper disable once DisposeOnUsingVariable
        zip.Dispose();

        // Act
        InstallationHelper.Install(installerFile);

        // Assert
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(metadata);
        var installedFile = new FileInfo(Path.Combine(installationDirectory.FullName, "Test.exe"));
        Assert.True(installedFile.Exists);
    }
}