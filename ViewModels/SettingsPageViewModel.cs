using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ControlzEx.Theming;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Enums;
using dev_flow.Interfaces;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls.Dialogs;

namespace dev_flow.ViewModels;

/// <summary>
/// View model for the Settings page.
/// </summary>
public class SettingsPageViewModel : ViewModelBase
{
    // Commands
    public ICommand ThemeChangedCommand { get; }
    public ICommand OpenExportLocation { get; }
    public ICommand ExportAllCommand { get; }
    public ICommand DeleteAllCommand { get; }

    // Properties

    public static bool IsDarkTheme => Settings.Default.Theme == ThemeEnum.DarkTheme;

    private string _defaultExportLocation;

    public string DefaultExportLocation
    {
        get => _defaultExportLocation;
        set
        {
            _defaultExportLocation = value;
            OnPropertyChanged(nameof(DefaultExportLocation));
        }
    }

    private readonly DialogCoordinator _dialogCoordinator;

    public SettingsPageViewModel()
    {
        LoadDefaultExportLocation();
        _dialogCoordinator = new DialogCoordinator();

        ThemeChangedCommand = new RelayCommand(OnThemeChanged);
        OpenExportLocation = new RelayCommand(OnOpenExportLocation);
        ExportAllCommand = new AsyncRelayCommand(async () => await RunExportAllWorkspaces());
        DeleteAllCommand = new AsyncRelayCommand(async () => await RunDeleteAllWorkspaces());
    }

    /// <summary>
    /// Recreates the workspace directory.
    /// </summary>
    private void RecreateWorkspaceDirectory()
    {
        // Get the directory where the application is running
        var currentDirectory = Settings.Default.WorkspacePath;

        // Combine the app directory path with the directory name
        var directoryPath = Path.Combine(currentDirectory, Constants.TopLevelDirectory);

        // Check if the directory exists
        if (!Directory.Exists(directoryPath))
        {
            try
            {
                // If not, create it
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine("Base directory created successfully.");
            }
            // Handle any exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating base directory: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Base directory already exists.");
        }
    }

    /// <summary>
    /// Deletes all workspaces asynchronously.
    /// </summary>
    private Task DeleteAllWorkspacesAsync()
    {
        var directoryPath = Constants.TopLevelDirectory;

        try
        {
            // Check if the directory exists
            if (Directory.Exists(directoryPath))
            {
                // Delete the directory and all its contents
                Directory.Delete(directoryPath, true);
            }

            // Clear the favourites list
            Settings.Default.Favourites.Clear();

            // Recreate the base workspace directory
            RecreateWorkspaceDirectory();
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Runs the delete all workspaces operation asynchronously.
    /// </summary>
    private async Task RunDeleteAllWorkspaces()
    {
        try
        {
            // Show a confirmation dialog
            var dialogResult =
                await _dialogCoordinator.ShowMessageAsync(this, "Delete Workspace",
                    "Deleting all workspaces cannot be undone, all data will be removed permanently.\n\nAre you sure you want to delete this workspace?",
                    MessageDialogStyle.AffirmativeAndNegative);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                var progressDialog = await _dialogCoordinator.ShowProgressAsync(this, "Deleting All Workspaces",
                    "All workspaces are being deleted. It may take a couple of minutes...");
                // Show a progress dialog
                progressDialog.SetIndeterminate();

                // Run the delete all workspaces operation
                await Task.Run(DeleteAllWorkspacesAsync);

                // Close the progress dialog
                await progressDialog.CloseAsync();
            }
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"Error running export directory: {ex.Message}");

            await _dialogCoordinator.ShowMessageAsync(this, "Error: Deleting All Workspaces",
                "An error occurred while deleting all workspaces.");
        }
    }

    /// <summary>
    /// Runs the export all workspaces operation asynchronously.
    /// </summary>
    private async Task RunExportAllWorkspaces()
    {
        try
        {
            // Get the export location from the settings or use the default location
            var exportLocation = !string.IsNullOrEmpty(Settings.Default.DefaultExportLocation)
                ? Settings.Default.DefaultExportLocation
                : AppDomain.CurrentDomain.BaseDirectory;

            var progressDialog = await _dialogCoordinator.ShowProgressAsync(this, "Exporting All Workspaces",
                $"All workspaces are being exported to: {exportLocation}. It may take a couple of minutes..." +
                $"\n\nIf the path is invalid, the default export path will be used: {AppDomain.CurrentDomain.BaseDirectory}");

            // Show a progress dialog
            progressDialog.SetIndeterminate();

            // Run the export all workspaces operation
            await Task.Run(ExportAllWorkspacesAsync);

            // Close the progress dialog
            await progressDialog.CloseAsync();
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"Error running export directory: {ex.Message}");

            await _dialogCoordinator.ShowMessageAsync(this, "Error: Exporting All Workspaces",
                "An error occurred while exporting all workspaces.");
        }
    }

    /// <summary>
    /// Exports all workspaces asynchronously.
    /// </summary>
    private Task ExportAllWorkspacesAsync()
    {
        var exportLocation = Settings.Default.DefaultExportLocation;
        var topLevelDirectory = Constants.TopLevelDirectory;
        string? zipFilePath = null;

        try
        {
            // The provided path is invalid, use a default path
            if (!string.IsNullOrEmpty(exportLocation) && !Directory.Exists(exportLocation))
            {
                exportLocation = Settings.Default.WorkspacePath;
            }

            zipFilePath = Path.Combine(exportLocation, $"{Path.GetFileName(topLevelDirectory)}.zip");

            // Create the zip file of the entire workspace directory
            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.EnumerateFiles(topLevelDirectory, "*", SearchOption.AllDirectories))
                {
                    zipArchive.CreateEntryFromFile(file,
                        Path.Combine(topLevelDirectory, file.Substring(topLevelDirectory.Length + 1)));
                }
            }
        }
        // Handle and log the exception
        catch (Exception ex)
        {
            Debug.WriteLine($"Error exporting directory: {ex.Message}");

            // Delete the potentially corrupted zip file
            if (!string.IsNullOrEmpty(zipFilePath) && File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens the export location.
    /// </summary>
    private void OnOpenExportLocation()
    {
        try
        {
            // Show a folder browser dialog
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DefaultExportLocation = dialog.SelectedPath;

                SaveDefaultExportLocation();
            }
        }
        // Handle and log the exception
        catch (Exception ex)
        {
            Console.WriteLine($"Error browsing folder: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the default export location into the DefaultExportLocation property.
    /// </summary>
    private void LoadDefaultExportLocation()
    {
        DefaultExportLocation = Settings.Default.DefaultExportLocation;
    }

    /// <summary>
    /// Saves the default export location to settings.
    /// </summary>
    private void SaveDefaultExportLocation()
    {
        Settings.Default.DefaultExportLocation = DefaultExportLocation;
        Settings.Default.Save();
    }


    /// <summary>
    /// Changes the theme.
    /// </summary>
    private void OnThemeChanged()
    {
        var currentThemeName = ThemeManager.Current.DetectTheme()?.Name;

        if (currentThemeName != null)
        {
            // Change the theme based on the current theme to light/dark mode
            if (currentThemeName == "Light.Steel")
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Steel");
                Settings.Default.Theme = ThemeEnum.DarkTheme;
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light.Steel");
                Settings.Default.Theme = ThemeEnum.LightTheme;
            }

            Settings.Default.Save();
        }
    }
}