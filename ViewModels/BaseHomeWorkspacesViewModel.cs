using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Helpers;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic.FileIO;
using Constants = dev_flow.Interfaces.Constants;
using SearchOption = System.IO.SearchOption;

namespace dev_flow.ViewModels;

/// <summary>
/// View model for the Home Workspaces page.
/// </summary>
public class BaseHomeWorkspacesViewModel : ViewModelBase
{
    private readonly IDialogCoordinator? _dialogCoordinator;

    private int _workspaceIdIncrement;

    // Commands
    public ICommand AddWorkspaceCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ImportWorkspaceCommand { get; }

    // Properties
    private RangedObservableCollection<WorkspaceItem> _newDisplayCards;

    private RangedObservableCollection<WorkspaceItem> _workspaceCards;

    public RangedObservableCollection<WorkspaceItem> WorkspaceCards
    {
        get => _workspaceCards;
        set
        {
            _workspaceCards = value;
            OnPropertyChanged(nameof(WorkspaceCards));
        }
    }

    private RangedObservableCollection<WorkspaceItem> _displayedWorkspaceCards;

    public RangedObservableCollection<WorkspaceItem> DisplayedWorkspaceCards
    {
        get => _displayedWorkspaceCards;
        set
        {
            _displayedWorkspaceCards = value;
            OnPropertyChanged(nameof(DisplayedWorkspaceCards));
        }
    }


    private string _workspaceSearchTerm;

    public string WorkspaceSearchTerm
    {
        get { return _workspaceSearchTerm; }
        set
        {
            if (_workspaceSearchTerm != value)
            {
                _workspaceSearchTerm = value;
                OnPropertyChanged(nameof(WorkspaceSearchTerm));
            }
        }
    }

    private bool _isMainCollectionEmpty;

    public bool IsMainCollectionEmpty
    {
        get { return _isMainCollectionEmpty; }
        set
        {
            if (_isMainCollectionEmpty != value)
            {
                _isMainCollectionEmpty = value;
                OnPropertyChanged(nameof(IsMainCollectionEmpty));
            }
        }
    }

    private bool _isDisplayCollectionEmpty;

    public bool IsDisplayCollectionEmpty
    {
        get { return _isDisplayCollectionEmpty; }
        set
        {
            if (_isDisplayCollectionEmpty != value)
            {
                _isDisplayCollectionEmpty = value;
                OnPropertyChanged(nameof(IsDisplayCollectionEmpty));
            }
        }
    }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    private bool _isFavouritesPage;

    public BaseHomeWorkspacesViewModel()
    {
        _workspaceCards = new RangedObservableCollection<WorkspaceItem>();
        _displayedWorkspaceCards = new RangedObservableCollection<WorkspaceItem>();
        _newDisplayCards = new RangedObservableCollection<WorkspaceItem>();


        _dialogCoordinator = new DialogCoordinator();
        AddWorkspaceCommand = new RelayCommand(OnAddWorkspace);
        SearchCommand = new AsyncRelayCommand(async () => await UpdateDisplayedCardsAsync());
        ImportWorkspaceCommand = new AsyncRelayCommand(async () => await OnImportWorkspace());
    }

    /// <summary>
    /// Imports a workspace from an open folder dialog.
    /// </summary>
    private async Task<Task> OnImportWorkspace()
    {
        try
        {
            // Open a folder browser dialog for the user to select the top-level directory
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "Select the workspace to import";

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var topLevelImportDirectory = folderDialog.SelectedPath;
                var isFileStructureValid = true;

                // Validate the presence of the "markdown.md" file
                var markdownFilePath = Path.Combine(topLevelImportDirectory, "markdown.md");
                if (!File.Exists(markdownFilePath))
                {
                    MessageBox.Show("The selected directory does not contain a 'markdown.md' file.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    isFileStructureValid = false;
                }

                // Validate the presence of the "Documents" directory
                var documentsDirectory = Path.Combine(topLevelImportDirectory, "Documents");
                if (!Directory.Exists(documentsDirectory))
                {
                    MessageBox.Show("The selected directory does not contain a 'Documents' directory.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    isFileStructureValid = false;
                }

                if (isFileStructureValid)
                {
                    // Perform the import asynchronously
                    await Task.Run(() =>
                    {
                        var destinationBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                            Constants.TopLevelDirectory);

                        var importDirectoryName = Path.GetFileName(topLevelImportDirectory);
                        var destinationDirectory = Path.Combine(destinationBaseDirectory, importDirectoryName);
                        var destinationDirectoryFullPath = Path.GetFullPath(destinationDirectory);

                        Directory.CreateDirectory(destinationDirectory);
                        FileSystem.CopyDirectory(topLevelImportDirectory, destinationDirectory, false);

                        // Add the imported workspace to the collection
                        WorkspaceCards.Add(new WorkspaceItem(new WorkspaceModel()
                        {
                            Name = importDirectoryName,
                            IsFavourite = false,
                            FullWorkspacePath = destinationDirectoryFullPath,
                            IsVisible = false,
                            DateModified = DateTime.Now,
                            ID = _workspaceIdIncrement
                        }, this));

                        _workspaceIdIncrement++;
                    });

                    MessageBox.Show("Directory imported successfully.", "Import Complete", MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    await UpdateDisplayedCardsAsync();
                }
            }
        }
        // Handle any exceptions that occur during the import process
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred during the import process: {ex.Message}", "Import Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Runs the export workspace process.
    /// </summary>
    /// <param name="workspaceItem">The workspace to be exported</param>
    public async Task OnExportWorkspaceRun(WorkspaceItem workspaceItem)
    {
        try
        {
            // Gets the export location from the settings or uses the default export location
            var exportLocation = !string.IsNullOrEmpty(Settings.Default.DefaultExportLocation)
                ? Settings.Default.DefaultExportLocation
                : AppDomain.CurrentDomain.BaseDirectory;

            // Show a progress dialog while the workspace is being exported
            var progressDialog = await _dialogCoordinator.ShowProgressAsync(this, "Exporting Workspace",
                $"{workspaceItem.Name} workspace is being exported to: {exportLocation}. It may take a couple of minutes..." +
                $"\n\nIf the path is invalid, the default export path will be used: {AppDomain.CurrentDomain.BaseDirectory}");

            // Show the progress dialog
            progressDialog.SetIndeterminate();

            // Run the export process asynchronously
            await ExportWorkspacesAsync(workspaceItem);

            // Close the progress dialog
            await progressDialog.CloseAsync();
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            Console.WriteLine($"Error running export directory: {ex.Message}");

            await _dialogCoordinator.ShowMessageAsync(this, "Error: Exporting Workspace",
                "An error occurred while exporting workspace.");
        }
    }

    /// <summary>
    /// Exports the workspace to a zip file.
    /// </summary>
    /// <param name="workspaceItem">The workspace to be exported.</param>
    private Task ExportWorkspacesAsync(WorkspaceItem workspaceItem)
    {
        var exportLocation = Settings.Default.DefaultExportLocation;
        var topLevelWorkspaceDirectory = workspaceItem.TrimmedBaseWorkspacePath;
        string? zipFilePath = null;

        try
        {
            // The provided path is invalid, use a default path
            if (!string.IsNullOrEmpty(exportLocation) && !Directory.Exists(exportLocation))
            {
                exportLocation = Settings.Default.WorkspacePath;
            }

            // Create the zip file
            zipFilePath = Path.Combine(exportLocation, $"{Path.GetFileName(topLevelWorkspaceDirectory)}.zip");

            // Create the zip archive and add the workspace files
            using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                var documentsDirectory = Path.Combine(topLevelWorkspaceDirectory, "Documents");
                var markdownFile = Path.Combine(topLevelWorkspaceDirectory, "markdown.md");

                if (Directory.Exists(documentsDirectory))
                {
                    // Add all the files in the "Documents" directory to the zip file
                    foreach (var file in Directory.EnumerateFiles(documentsDirectory, "*", SearchOption.AllDirectories))
                    {
                        var entryName = Path.Combine("Documents", file.Substring(documentsDirectory.Length + 1));
                        zipArchive.CreateEntryFromFile(file,
                            Path.Combine(Path.GetFileName(topLevelWorkspaceDirectory), entryName));
                    }
                }

                // Add the markdown file to the zip file
                if (File.Exists(markdownFile))
                {
                    zipArchive.CreateEntryFromFile(markdownFile,
                        Path.Combine(Path.GetFileName(topLevelWorkspaceDirectory), "markdown.md"));
                }
            }
        }
        // Handle and log the exception
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting directory: {ex.Message}");

            // Delete corrupted zip file if one was made
            if (!string.IsNullOrEmpty(zipFilePath) && File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the displayed cards based on the search term.
    /// </summary>
    private async Task<Task> UpdateDisplayedCardsAsync()
    {
        IsLoading = true;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _newDisplayCards.Clear();

            // Filter the workspace cards based on the search term
            var filteredCards = string.IsNullOrEmpty(WorkspaceSearchTerm)
                ? WorkspaceCards
                : WorkspaceCards.Where(card =>
                    card.LowerCaseName.ToLower().Contains(WorkspaceSearchTerm.ToLower()));

            // Add the filtered cards to the display collection based on the page type
            if (_isFavouritesPage)
            {
                _newDisplayCards.AddRange(string.IsNullOrEmpty(WorkspaceSearchTerm)
                    ? filteredCards.OrderByDescending(workspace => workspace.DateModified).Take(20)
                        .Where(workspace => workspace.IsFavourite)
                    : filteredCards.Take(20).Where(workspace => workspace.IsFavourite));
            }
            else
            {
                _newDisplayCards.AddRange(string.IsNullOrEmpty(WorkspaceSearchTerm)
                    ? filteredCards.OrderByDescending(workspace => workspace.DateModified).Take(5)
                    : filteredCards.Take(20));
            }

            DisplayedWorkspaceCards = _newDisplayCards;

            // Check if the main collection is empty
            if (!string.IsNullOrEmpty(WorkspaceSearchTerm))
            {
                IsMainCollectionEmpty = false;
                CheckDisplayCollectionBodyTextVisibility();
            }
            else
            {
                IsDisplayCollectionEmpty = false;
                CheckMainCollectionBodyTextVisibility();
            }
        });
        IsLoading = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if the main collection is empty and updates the visibility of the body text.
    /// </summary>
    private void CheckMainCollectionBodyTextVisibility()
    {
        IsMainCollectionEmpty = WorkspaceCards.Count == 0;
    }

    /// <summary>
    /// Checks if the display collection is empty and updates the visibility of the body text.
    /// </summary>
    private void CheckDisplayCollectionBodyTextVisibility()
    {
        IsDisplayCollectionEmpty = DisplayedWorkspaceCards.Count == 0;
    }

    /// <summary>
    /// Checks if the workspace name is already taken.
    /// </summary>
    /// <param name="workspaceName">The workspace name to check.</param>
    /// <returns>True if the workspace name is already taken, false otherwise.</returns>
    private bool CheckForDuplicateWorkspaceName(string workspaceName)
    {
        return WorkspaceCards.Any(workspace => workspace.Name == workspaceName);
    }

    /// <summary>
    /// Adds a new workspace.
    /// </summary>
    private async void OnAddWorkspace()
    {
        if (_dialogCoordinator != null)
        {
            var dialogResult =
                await _dialogCoordinator.ShowInputAsync(this, "Create a Workspace", "Enter Workspace Name:");

            if (!string.IsNullOrEmpty(dialogResult))
            {
                // Check if the workspace name is already taken
                var isDuplicateWorkspaceName = CheckForDuplicateWorkspaceName(dialogResult);

                if (!isDuplicateWorkspaceName)
                {
                    // Create a new workspace item
                    var newWorkspace = new WorkspaceItem(new WorkspaceModel()
                    {
                        Name = dialogResult,
                        IsFavourite = false,
                        FullWorkspacePath = string.Empty,
                        IsVisible = false,
                        DateModified = DateTime.Now,
                        ID = _workspaceIdIncrement
                    }, this);

                    var workspacePath = Path.Combine(Constants.TopLevelDirectory, dialogResult);

                    try
                    {
                        await Task.Run(() =>
                        {
                            // Create the workspace directory files
                            Directory.CreateDirectory(workspacePath);

                            // Create the "markdown.md" file
                            var markdownFilePath = Path.Combine(workspacePath, "markdown.md");
                            File.WriteAllText(markdownFilePath, string.Empty);

                            // Create the "Name+Documents" subdirectory
                            var documentsDirectoryPath = Path.Combine(workspacePath, "Documents");
                            Directory.CreateDirectory(documentsDirectoryPath);
                        });

                        newWorkspace.FullWorkspacePath = workspacePath;

                        Console.WriteLine("Directory and files created successfully: {0}", workspacePath);

                        // Increment the ID and add the new workspace to the collection
                        _workspaceIdIncrement++;
                        WorkspaceCards.Add(newWorkspace);

                        // Update the displayed cards and check the visibility of the body text
                        await UpdateDisplayedCardsAsync();
                        CheckMainCollectionBodyTextVisibility();
                    }
                    // Handle any exceptions
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating directory: {0}", ex.Message);
                        await _dialogCoordinator.ShowMessageAsync(this, "Directory Creation Failed",
                            "Error creating directory. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Error creating directory: Name already taken.");
                    await _dialogCoordinator.ShowMessageAsync(this, "Invalid Directory Name",
                        "Directory name is already taken.");
                }
            }
            else
            {
                Console.WriteLine("Error creating directory: Name is empty or process was cancelled.");
                await _dialogCoordinator.ShowMessageAsync(this, "Directory Creation Failed",
                    "Directory name is empty or process was cancelled.");
            }
        }
    }

    /// <summary>
    /// Gets the workspaces from the directory.
    /// </summary>
    private async Task GetWorkspacesFromDirectoryAsync()
    {
        IsLoading = true;

        try
        {
            // Get the current directory and the root directory
            var currentDirectory = Settings.Default.WorkspacePath;
            var rootDirectory = Path.Combine(currentDirectory, Constants.TopLevelDirectory);
            if (Directory.Exists(rootDirectory))
            {
                var subDirectories = await Task.Run(() =>
                    Directory.GetDirectories(rootDirectory, "*", SearchOption.TopDirectoryOnly));

                // Add each workspace to the collection
                foreach (var subDirectory in subDirectories)
                {
                    var workspaceName = Path.GetFileName(subDirectory);
                    var workspaceFullPath = Path.GetFullPath(subDirectory);
                    var workspaceLastWriteTime = File.GetLastWriteTime(subDirectory);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        WorkspaceCards.Add(new WorkspaceItem(new WorkspaceModel()
                        {
                            Name = workspaceName,
                            IsFavourite = GetFavourites(workspaceName),
                            FullWorkspacePath = workspaceFullPath,
                            IsVisible = false,
                            DateModified = workspaceLastWriteTime,
                            ID = _workspaceIdIncrement
                        }, this));
                    });

                    // Increment the workspace ID
                    _workspaceIdIncrement++;
                }
            }
        }
        // Handle any exceptions
        catch (NullReferenceException ex)
        {
            Console.WriteLine("Error getting directories: {0}", ex.Message);
        }
    }

    /// <summary>
    /// Checks if the workspace name is in the favourites list.
    /// </summary>
    /// <param name="workspaceName">The name of the workspace to check.</param>
    /// <returns>True if the workspace name is in the favourites list, false otherwise.</returns>
    private bool GetFavourites(string workspaceName)
    {
        return Settings.Default.Favourites.Contains(workspaceName);
    }

    /// <summary>
    /// Handles the tab click event.
    /// </summary>
    /// <param name="isFavouritesPage">Indicates whether the favourites page is currently selected.</param>
    public async void HandleTabClick(bool isFavouritesPage = false)
    {
        IsLoading = true;
        _isFavouritesPage = isFavouritesPage;

        // Load the workspaces if the collection is empty
        if (WorkspaceCards.Count == 0)
        {
            await Task.Run(GetWorkspacesFromDirectoryAsync);
        }

        // Update the displayed cards based on the search term
        if (string.IsNullOrEmpty(WorkspaceSearchTerm))
        {
            DisplayedWorkspaceCards.Clear();
            var sortedWorkspaces = WorkspaceCards.OrderByDescending(workspace => workspace.DateModified);

            DisplayedWorkspaceCards.AddRange(_isFavouritesPage
                ? sortedWorkspaces.Take(20).Where(workspace => workspace.IsFavourite)
                : sortedWorkspaces.Take(5));

            CheckMainCollectionBodyTextVisibility();
        }
        else
        {
            await UpdateDisplayedCardsAsync();
            CheckDisplayCollectionBodyTextVisibility();
        }

        IsLoading = false;
    }

    /// <summary>
    /// Deletes a workspace.
    /// </summary>
    /// <param name="workspaceItem">The workspace to delete.</param>
    public async void DeleteWorkspace(WorkspaceItem workspaceItem)
    {
        if (_dialogCoordinator != null)
        {
            // Show a confirmation dialog before deleting the workspace
            var dialogResult =
                await _dialogCoordinator.ShowMessageAsync(this, "Delete Workspace",
                    "Deleting a workspace cannot be undone.\n\nAre you sure you want to delete this workspace?",
                    MessageDialogStyle.AffirmativeAndNegative);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                try
                {
                    IsLoading = true;

                    // Asynchronously delete the workspace directory and remove the workspace from the collection
                    await Task.Run(() => Directory.Delete(workspaceItem.FullWorkspacePath, true));
                    WorkspaceCards.Remove(workspaceItem);

                    // Remove the workspace from the favourites list if present
                    Settings.Default.Favourites.Remove(workspaceItem.Name);
                    Settings.Default.Save();

                    // Update the displayed cards and check the visibility of the body text
                    await UpdateDisplayedCardsAsync();
                    CheckMainCollectionBodyTextVisibility();
                }
                // Handle any exceptions
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting directory: {0}", ex.Message);
                }
            }
        }

        IsLoading = false;
    }

    /// <summary>
    /// Edits the name of a workspace.
    /// </summary>
    /// <param name="workspaceItem">The workspace whose name is to be edited.</param>
    public async void EditWorkspaceName(WorkspaceItem workspaceItem)
    {
        IsLoading = true;

        // Input dialog to get the new workspace name
        var dialogResult =
            await _dialogCoordinator.ShowInputAsync(this, "Edit Workspace Name", "Rename Workspace:");

        if (!string.IsNullOrEmpty(dialogResult))
        {
            var previousDirectoryPath = workspaceItem.TrimmedBaseWorkspacePath;
            var newDirectoryPath = Path.Combine(Constants.TopLevelDirectory, dialogResult);

            try
            {
                if (!(string.IsNullOrEmpty(previousDirectoryPath) || string.IsNullOrEmpty(newDirectoryPath)))
                {
                    var isDuplicateWorkspaceName = CheckForDuplicateWorkspaceName(dialogResult);

                    // Check if the new directory name is different and not a duplicate
                    if (previousDirectoryPath != newDirectoryPath && !isDuplicateWorkspaceName)
                    {
                        // Check if the previous directory already exists
                        if (Directory.Exists(previousDirectoryPath))
                        {
                            var isPreviousWorkspaceFavourite = workspaceItem.IsFavourite;

                            // Remove from Favourites
                            Settings.Default.Favourites.Remove(workspaceItem.Name);

                            // Move the directory to the new path
                            Directory.Move(previousDirectoryPath, newDirectoryPath);
                            workspaceItem.Name = dialogResult;
                            workspaceItem.FullWorkspacePath = Path.GetFullPath(newDirectoryPath);

                            // Add to Favourites
                            workspaceItem.IsFavourite = isPreviousWorkspaceFavourite;
                            Settings.Default.Favourites.Add(workspaceItem.Name);

                            // Update the workspace date modified and the directory last write time
                            workspaceItem.UpdateWorkspaceDateModified();
                            Directory.SetLastWriteTime(newDirectoryPath, DateTime.Now);

                            // Update the displayed cards and save the settings
                            await UpdateDisplayedCardsAsync();
                            Settings.Default.Save();
                        }
                    }
                    else
                    {
                        await _dialogCoordinator.ShowMessageAsync(this, "Invalid Directory Name",
                            "Cannot rename directory: Name already taken or the same.");
                    }
                }
            }
            // Handle any exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"Error renaming directory: {ex.Message}");
            }
        }

        IsLoading = false;
    }

    /// <summary>
    /// Toggles the favourite status of a workspace.
    /// </summary>
    /// <param name="workspaceItem">The workspace whose favourite status is to be toggled.</param>
    public async void FavouriteToggled(WorkspaceItem workspaceItem)
    {
        IsLoading = true;
        if (workspaceItem.IsFavourite)
        {
            Settings.Default.Favourites.Remove(workspaceItem.Name);
            workspaceItem.IsFavourite = false;
        }
        else
        {
            Settings.Default.Favourites.Add(workspaceItem.Name);
            workspaceItem.IsFavourite = true;
        }

        Settings.Default.Save();

        await UpdateDisplayedCardsAsync();

        IsLoading = false;
    }
}