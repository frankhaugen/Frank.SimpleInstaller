using Frank.SimpleInstaller.Helpers;

using Spectre.Console;

namespace Frank.SimpleInstaller.Cli.Helpers
{
    public class FileExplorer
    {
        private static bool DisplayIcons => AnsiConsole.Profile.Capabilities.Unicode;
        private bool IsWindows { get; } = Environment.OSVersion.Platform.ToString().StartsWith("Win", StringComparison.OrdinalIgnoreCase);
        private static int PageSize => 15;
        private static bool CanCreateFolder => true;
        private static string LevelUpText => "Go to parent folder";
        private static string MoreChoicesText => "Use arrows Up and Down to select";
        private static string CreateNewText => "Create new folder";
        private static string SelectFileText => "Select File";
        private static string SelectFolderText => "Select Folder";
        private static string SelectDriveText => "Select Drive";
        private static string SelectActualText => "Select Current Folder";

        private enum FileSystemItemVariant
        {
            Drive,
            Folder,
            File,
            CreateNew,
            SelectActual,
            UpOneLevel
        }
        
        private class FileSystemItem
        {
            public string DisplayName { get; }
            public FileSystemInfo Info { get; }
            
            public FileSystemItemVariant Variant { get; }

            public FileSystemItem(string displayName, FileSystemInfo info, FileSystemItemVariant variant)
            {
                DisplayName = displayName;
                Info = info;
                Variant = variant;
            }

            public string GetDisplayMarkupString()
            {
                string icon = GetIcon();
                string color = GetColor();
                return DisplayIcons ? $"{icon} [{color}]{DisplayName}[/]" : $"[{color}]{DisplayName}[/]";
            }
            
            private string GetColor() =>
                Variant switch
                {
                    FileSystemItemVariant.Drive => "green",
                    FileSystemItemVariant.CreateNew => "green",
                    FileSystemItemVariant.SelectActual => "green",
                    FileSystemItemVariant.UpOneLevel => "green",
                    FileSystemItemVariant.Folder => "white",
                    FileSystemItemVariant.File => "grey",
                    _ => "white"
                };

            private string GetIcon() =>
                Variant switch
                {
                    FileSystemItemVariant.Drive => "💾",
                    FileSystemItemVariant.Folder => "📁",
                    FileSystemItemVariant.File => "📄",
                    FileSystemItemVariant.CreateNew => "➕",
                    FileSystemItemVariant.SelectActual => "📌",
                    FileSystemItemVariant.UpOneLevel => "⬆️",
                    _ => "❔"
                };
        }

        public Task<FileInfo> GetFilePathAsync(DirectoryInfo actualFolder) => GetPathAsync<FileInfo>(actualFolder, true);

        public Task<FileInfo> GetFilePathAsync() => GetFilePathAsync(DirectoryHelper.GetBaseDirectory());

        public Task<DirectoryInfo> GetFolderPathAsync(DirectoryInfo actualFolder) => GetPathAsync<DirectoryInfo>(actualFolder, false);

        public Task<DirectoryInfo> GetFolderPathAsync() => GetFolderPathAsync(DirectoryHelper.GetBaseDirectory());

        private async Task<T> GetPathAsync<T>(DirectoryInfo actualFolder, bool selectFile) where T : FileSystemInfo
        {
            return await GetPathRecursiveAsync<T>(actualFolder, actualFolder, selectFile);
        }

        private async Task<T> GetPathRecursiveAsync<T>(DirectoryInfo actualFolder, DirectoryInfo lastFolder, bool selectFile) where T : FileSystemInfo
        {
            try
            {
                Directory.SetCurrentDirectory(actualFolder.FullName);
            }
            catch
            {
                actualFolder = lastFolder;
                Directory.SetCurrentDirectory(actualFolder.FullName);
            }

            var items = await GetFileSystemItemsAsync(actualFolder, selectFile);

            var selectedItem = PromptSelection(selectFile, items);

            return selectedItem switch
            {
                FileSystemItem item when item.Variant == FileSystemItemVariant.Drive && item.DisplayName == SelectDriveText => await GetPathRecursiveAsync<T>(SelectDrive(), actualFolder, selectFile),
                FileSystemItem item when item.Variant == FileSystemItemVariant.CreateNew && item.DisplayName == CreateNewText => await GetPathRecursiveAsync<T>(await CreateNewFolderAsync(actualFolder), actualFolder, selectFile),
                FileSystemItem item when item.Variant == FileSystemItemVariant.SelectActual && !selectFile => (T)(FileSystemInfo)actualFolder,
                FileSystemItem item when item.Info is DirectoryInfo dir => await GetPathRecursiveAsync<T>(dir, actualFolder, selectFile),
                FileSystemItem item when item.Info is FileInfo file => (T)(FileSystemInfo)file,
                _ => throw new InvalidOperationException("Unexpected path type")
            };
        }

        private async Task<List<FileSystemItem>> GetFileSystemItemsAsync(DirectoryInfo actualFolder, bool selectFile)
        {
            var items = new List<FileSystemItem>();

            if (IsWindows)
                items.Add(new FileSystemItem(SelectDriveText, null, FileSystemItemVariant.Drive));

            if (actualFolder.Parent != null)
                items.Add(new FileSystemItem(LevelUpText, actualFolder.Parent, FileSystemItemVariant.UpOneLevel));

            if (!selectFile)
                items.Add(new FileSystemItem(SelectActualText, actualFolder, FileSystemItemVariant.SelectActual));

            if (CanCreateFolder)
                items.Add(new FileSystemItem(CreateNewText, null, FileSystemItemVariant.CreateNew));

            var directories = await Task.Run(actualFolder.GetDirectories);
            items.AddRange(directories.Select(dir => new FileSystemItem(dir.Name, dir, FileSystemItemVariant.Folder)));

            if (selectFile)
            {
                var files = await Task.Run(() => actualFolder.GetFiles());
                items.AddRange(files.Select(file => new FileSystemItem(file.Name, file, FileSystemItemVariant.File)));
            }

            return items;
        }

        private static FileSystemItem PromptSelection(bool selectFile, List<FileSystemItem> items)
        {
            var title = selectFile ? SelectFileText : SelectFolderText;

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<FileSystemItem>()
                    .Title($"[green]{title} ({Directory.GetCurrentDirectory()}):[/]")
                    .PageSize(PageSize)
                    .MoreChoicesText($"[grey]{MoreChoicesText}[/]")
                    .AddChoices(items)
                    .UseConverter(choice => choice.GetDisplayMarkupString())
            );

            return choice;
        }

        private static DirectoryInfo SelectDrive()
        {
            var drives = Directory.GetLogicalDrives();
            var driveItems = drives.Select(drive => new FileSystemItem(drive, new DirectoryInfo(drive), FileSystemItemVariant.Drive)).ToList();

            AnsiConsole.Clear();
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[green]{SelectDriveText}[/]").Centered());

            AnsiConsole.WriteLine();
            var selectedDrive = PromptSelection(false, driveItems);

            return (DirectoryInfo)selectedDrive.Info;
        }

        private static async Task<DirectoryInfo> CreateNewFolderAsync(DirectoryInfo currentFolder)
        {
            var folderName = AnsiConsole.Ask<string>($"[blue]{CreateNewText}: [/]");
            if (string.IsNullOrEmpty(folderName)) return currentFolder;

            try
            {
                var newFolder = new DirectoryInfo(Path.Combine(currentFolder.FullName, folderName));
                await Task.Run(() => newFolder.Create());
                return newFolder;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"[red]Error: {ex.Message}[/]");
                return currentFolder;
            }
        }
    }
}
