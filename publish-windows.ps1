# Purpose: Publish the application for Windows

# Define target frameworks and runtime identifiers
$artifactsDir = ".artifacts"
$framework = "net8.0"
$runtime = "win-x64"
$publishDir = "publish"
$fullPublishDir = "$artifactsDir/$framework/$runtime/$publishDir"
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$projectFile = "Frank.SimpleInstaller.Cli/Frank.SimpleInstaller.Cli.csproj"

# Change to the script directory
Set-Location $scriptDir

# Clean publish directory if it exists
if (Test-Path $fullPublishDir)
{
    Write-Host "Cleaning publish directory $fullPublishDir"
    Remove-Item -Recurse -Force $fullPublishDir
}

# Create publish directory
New-Item -ItemType Directory -Force -Path $fullPublishDir

# Publish for Windows
Write-Host "Building for $framework on $runtime"
dotnet publish $projectFile -c Release -o $fullPublishDir --self-contained -r $runtime -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishReadyToRun=true

if ($LASTEXITCODE -ne 0)
{
    Write-Host "dotnet publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Build completed"