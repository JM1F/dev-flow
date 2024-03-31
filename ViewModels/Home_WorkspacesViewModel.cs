using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Helpers;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Constants = dev_flow.Interfaces.Constants;

namespace dev_flow.ViewModels;

public class Home_WorkspacesViewModel : ViewModelBase
{
    private IDialogCoordinator? _dialogCoordinator;

    private int _workspaceIDIncrement;

    private RangedObservableCollection<WorkspaceItem> _workspaceCards =
        new RangedObservableCollection<WorkspaceItem>();

    private RangedObservableCollection<WorkspaceItem> _displayedWorkspaceCards =
        new RangedObservableCollection<WorkspaceItem>();

    private RangedObservableCollection<WorkspaceItem> _newDisplayCards =
        new RangedObservableCollection<WorkspaceItem>();

    public ICommand AddWorkspaceCommand { get; }
    public RelayCommand WorkspaceEntryButtonClicked { get; private set; }


    public RangedObservableCollection<WorkspaceItem> WorkspaceCards
    {
        get => _workspaceCards;
        set
        {
            _workspaceCards = value;
            OnPropertyChanged(nameof(WorkspaceCards));
        }
    }

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
                UpdateDisplayedCards();
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

    private void UpdateDisplayedCards()
    {
        IsLoading = true;
        _newDisplayCards.Clear();

        var filteredCards = string.IsNullOrEmpty(WorkspaceSearchTerm)
            ? WorkspaceCards
            : WorkspaceCards.Where(card => card.LowerCaseName.ToLower().Contains(WorkspaceSearchTerm.ToLower()));

        if (_isFavouritesPage)
        {
            _newDisplayCards.AddRange(string.IsNullOrEmpty(WorkspaceSearchTerm)
                ? filteredCards.OrderByDescending(workspace => workspace.DateModified).Take(20)
                    .Where(workspace => workspace.IsFavorite)
                : filteredCards.Take(20).Where(workspace => workspace.IsFavorite));
        }
        else
        {
            _newDisplayCards.AddRange(string.IsNullOrEmpty(WorkspaceSearchTerm)
                ? filteredCards.OrderByDescending(workspace => workspace.DateModified).Take(5)
                : filteredCards.Take(20));
        }

        DisplayedWorkspaceCards = _newDisplayCards;

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

        IsLoading = false;
    }

    private void CheckMainCollectionBodyTextVisibility()
    {
        IsMainCollectionEmpty = WorkspaceCards.Count == 0;
    }

    private void CheckDisplayCollectionBodyTextVisibility()
    {
        IsDisplayCollectionEmpty = DisplayedWorkspaceCards.Count == 0;
    }

    public Home_WorkspacesViewModel()
    {
        _dialogCoordinator = new DialogCoordinator();
        AddWorkspaceCommand = new RelayCommand(OnAddWorkspace);
    }

    private bool CheckForDuplicateWorkspaceName(string workspaceName)
    {
        return WorkspaceCards.Any(workspace => workspace.Name == workspaceName);
    }

    private async void OnAddWorkspace()
    {
        if (_dialogCoordinator != null)
        {
            var dialogResult =
                await _dialogCoordinator.ShowInputAsync(this, "Create a Workspace", "Enter Workspace Name:");

            if (!string.IsNullOrEmpty(dialogResult))
            {
                var isDuplicateWorkspaceName = CheckForDuplicateWorkspaceName(dialogResult);

                if (!isDuplicateWorkspaceName)
                {
                    var newWorkspace = new WorkspaceItem(new WorkspaceModel()
                    {
                        Name = dialogResult,
                        IsFavorite = false,
                        FullWorkspacePath = string.Empty,
                        IsVisible = false,
                        DateModified = DateTime.Now,
                        ID = _workspaceIDIncrement
                    }, this);

                    var workspacePath = Path.Combine(Constants.TopLevelDirectory, dialogResult);

                    try
                    {
                        await Task.Run(() => Directory.CreateDirectory(workspacePath));
                        newWorkspace.FullWorkspacePath = workspacePath;

                        Console.WriteLine("Directory created successfully: {0}", workspacePath);

                        _workspaceIDIncrement++;
                        WorkspaceCards.Add(newWorkspace);

                        UpdateDisplayedCards();
                        CheckMainCollectionBodyTextVisibility();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating directory: {0}", ex.Message);
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

    private async Task GetWorkspacesFromDirectoryAsync()
    {
        try
        {
            var currentDirectory = Settings.Default.WorkspacePath;
            var rootDirectory = Path.Combine(currentDirectory, Constants.TopLevelDirectory);
            if (Directory.Exists(rootDirectory))
            {
                var subDirectories = await Task.Run(() =>
                    Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories));

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
                            IsFavorite = GetFavourites(workspaceName),
                            FullWorkspacePath = workspaceFullPath,
                            IsVisible = false,
                            DateModified = workspaceLastWriteTime,
                            ID = _workspaceIDIncrement
                        }, this));
                    });

                    _workspaceIDIncrement++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting workspaces: {ex.Message}");
        }
    }

    private bool GetFavourites(string workspaceName)
    {
        return Settings.Default.Favourites.Contains(workspaceName);
    }

    public async void HandleTabClick(bool isFavouritesPage = false)
    {
        IsLoading = true;
        _isFavouritesPage = isFavouritesPage;

        if (WorkspaceCards.Count == 0)
        {
            await Task.Run(GetWorkspacesFromDirectoryAsync);
        }

        if (string.IsNullOrEmpty(WorkspaceSearchTerm))
        {
            DisplayedWorkspaceCards.Clear();
            var sortedWorkspaces = WorkspaceCards.OrderByDescending(workspace => workspace.DateModified);

            DisplayedWorkspaceCards.AddRange(_isFavouritesPage
                ? sortedWorkspaces.Take(20).Where(workspace => workspace.IsFavorite)
                : sortedWorkspaces.Take(5));

            CheckMainCollectionBodyTextVisibility();
        }
        else
        {
            UpdateDisplayedCards();
            CheckDisplayCollectionBodyTextVisibility();
        }

        IsLoading = false;
    }

    public async void DeleteWorkspace(WorkspaceItem workspaceItem)
    {
        if (_dialogCoordinator != null)
        {
            var dialogResult =
                await _dialogCoordinator.ShowMessageAsync(this, "Delete Workspace",
                    "Deleting a workspace cannot be undone.\n\nAre you sure you want to delete this workspace?",
                    MessageDialogStyle.AffirmativeAndNegative);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                try
                {
                    await Task.Run(() => Directory.Delete(workspaceItem.FullWorkspacePath, true));
                    WorkspaceCards.Remove(workspaceItem);

                    Settings.Default.Favourites.Remove(workspaceItem.Name);
                    Settings.Default.Save();

                    UpdateDisplayedCards();
                    CheckMainCollectionBodyTextVisibility();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting directory: {0}", ex.Message);
                }
            }
        }
    }

    public async void EditWorkspaceName(WorkspaceItem workspaceItem)
    {
        var dialogResult =
            await _dialogCoordinator.ShowInputAsync(this, "Edit Workspace Name", "Rename Workspace:");

        if (!string.IsNullOrEmpty(dialogResult))
        {
            var previousDirectoryPath = workspaceItem.TrimmedWorkspacePath;
            var newDirectoryPath = Path.Combine(Constants.TopLevelDirectory, dialogResult);

            try
            {
                if (!(string.IsNullOrEmpty(previousDirectoryPath) || string.IsNullOrEmpty(newDirectoryPath)))
                {
                    var isDuplicateWorkspaceName = CheckForDuplicateWorkspaceName(dialogResult);

                    if ((previousDirectoryPath != newDirectoryPath) && !isDuplicateWorkspaceName)
                    {
                        if (Directory.Exists(previousDirectoryPath))
                        {
                            var isPreviousWorkspaceFavourite = workspaceItem.IsFavorite;
                            // Remove
                            Settings.Default.Favourites.Remove(workspaceItem.Name);

                            // Add
                            Directory.Move(previousDirectoryPath, newDirectoryPath);
                            workspaceItem.Name = dialogResult;
                            workspaceItem.FullWorkspacePath = Path.GetFullPath(newDirectoryPath);

                            workspaceItem.IsFavorite = isPreviousWorkspaceFavourite;
                            Settings.Default.Favourites.Add(workspaceItem.Name);

                            workspaceItem.DateModified = DateTime.Now;
                            Directory.SetLastWriteTime(newDirectoryPath, DateTime.Now);

                            // Update & Save
                            UpdateDisplayedCards();
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error renaming directory: {ex.Message}");
            }
        }
    }

    public void FavouriteToggled(WorkspaceItem workspaceItem)
    {
        if (workspaceItem.IsFavorite)
        {
            Settings.Default.Favourites.Remove(workspaceItem.Name);
            workspaceItem.IsFavorite = false;
        }
        else
        {
            Settings.Default.Favourites.Add(workspaceItem.Name);
            workspaceItem.IsFavorite = true;
        }

        Settings.Default.Save();

        if (_isFavouritesPage)
        {
            UpdateDisplayedCards();
        }
    }
}