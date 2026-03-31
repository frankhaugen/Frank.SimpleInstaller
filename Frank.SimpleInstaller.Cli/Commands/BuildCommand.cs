using System.ComponentModel;

using Spectre.Console;
using Spectre.Console.Cli;

public class BuildCommand : AsyncCommand<BuildCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-c|--configuration <CONFIGURATION>")]
        [Description("Configuration to build")]
        [DefaultValue("Release")]
        public string? Configuration { get; set; }
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("Building application...");
        AnsiConsole.MarkupLine($"Configuration: [bold]{settings.Configuration}[/]");
        return 0;
    }
    
        public class BuildConfiguration
        {
            public string? TargetFramework { get; init; } = "net8.0";
            public string? Configuration { get; init; } = "Release";
            public string? PublishDir { get; init; }
            public bool? SelfContained { get; init; } = true;
            public bool? PublishSingleFile { get; init; } = true;
            public string? RuntimeIdentifier { get; init; } = "win-x64";
            public bool? PublishTrimmed { get; init; } = true;

            public Dictionary<string, string> ToMsBuildProperties()
            {
                var properties = new Dictionary<string, string>();

                if (TargetFramework is not null)
                    properties[nameof(TargetFramework)] = TargetFramework;
            
                if (Configuration is not null)
                    properties[nameof(Configuration)] = Configuration;
            
                if (PublishDir is not null)
                    properties[nameof(PublishDir)] = PublishDir;
            
                if (SelfContained.HasValue)
                    properties[nameof(SelfContained)] = SelfContained.Value.ToString().ToLower();
            
                if (PublishSingleFile.HasValue)
                    properties[nameof(PublishSingleFile)] = PublishSingleFile.Value.ToString().ToLower();
            
                if (RuntimeIdentifier is not null)
                    properties[nameof(RuntimeIdentifier)] = RuntimeIdentifier;
            
                if (PublishTrimmed.HasValue)
                    properties[nameof(PublishTrimmed)] = PublishTrimmed.Value.ToString().ToLower();

                return properties;
            }
        }

}