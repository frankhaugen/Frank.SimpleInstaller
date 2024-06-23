using Frank.SimpleInstaller.Helpers;
using Frank.SimpleInstaller.Models;

using OperatingSystem = Frank.SimpleInstaller.Models.OperatingSystem;

namespace Frank.SimpleInstaller.Tests;

public class TestOperatingSystemHelper
{
    [Fact]
    public void GetOperatingSystem_ReturnsValidOs()
    {
        // Arrange
        
        // Act
        var os = OperatingSystemHelper.GetOperatingSystem();

        // Assert
        Assert.NotEqual(OperatingSystem.Unknown, os);
    }

    [Fact]
    public void GetCommonApplicationDataDirectory_ReturnsCommonApplicationDataDirectory()
    {
        // Arrange

        // Act
        var commonApplicationDataDirectory = OperatingSystemHelper.GetCommonApplicationDataDirectory();

        // Assert
        Assert.True(commonApplicationDataDirectory.Exists);
    }

    [Fact]
    public void GetInstallationDirectory_ReturnsInstallationDirectory()
    {
        // Arrange
        var metadata = new InstallationMetadata { Name = "Test" };

        // Act
        var installationDirectory = OperatingSystemHelper.GetInstallationDirectory(metadata);

        // Assert
        Assert.True(installationDirectory.Exists);
    }

    [Fact]
    public void GetStartMenuApplicationDirectory_ReturnsStartMenuApplicationDirectory()
    {
        // Arrange
        
        var metadata = new InstallationMetadata { Name = "Test" };

        // Act
        var startMenuApplicationDirectory = OperatingSystemHelper.GetStartMenuApplicationDirectory(metadata);

        // Assert
        Assert.True(startMenuApplicationDirectory.Exists);
    }

    [Fact]
    public void GetStartMenuDirectory_ReturnsStartMenuDirectory()
    {
        // Arrange

        // Act
        var startMenuDirectory = OperatingSystemHelper.GetStartMenuDirectory();

        // Assert
        Assert.True(startMenuDirectory.Exists);
    }

    [Fact]
    public void GetStartMenuProgramsDirectory_ReturnsStartMenuProgramsDirectory()
    {
        // Arrange

        // Act
        var startMenuProgramsDirectory = OperatingSystemHelper.GetStartMenuProgramsDirectory();

        // Assert
        Assert.True(startMenuProgramsDirectory.Exists);
    }
}