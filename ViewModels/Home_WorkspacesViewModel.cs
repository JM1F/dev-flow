using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Helpers;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Constants = dev_flow.Interfaces.Constants;

namespace dev_flow.ViewModels;

public class Home_WorkspacesViewModel : ViewModelBase
{
    private RangedObservableCollection<WorkspaceItem> _workspaceCards =
        new RangedObservableCollection<WorkspaceItem>();

    private RangedObservableCollection<WorkspaceItem> _displayedWorkspaceCards =
        new RangedObservableCollection<WorkspaceItem>();

    private RangedObservableCollection<WorkspaceItem> _newDisplayCards =
        new RangedObservableCollection<WorkspaceItem>();

    public ICommand AddWorkspaceCommand { get; }

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

    private void UpdateDisplayedCards()
    {
        IsLoading = true;
        _newDisplayCards.Clear();

        var filteredCards = string.IsNullOrEmpty(WorkspaceSearchTerm)
            ? WorkspaceCards
            : WorkspaceCards.Where(card => card.Name.ToLower().Contains(WorkspaceSearchTerm.ToLower()));

        _newDisplayCards.AddRange(string.IsNullOrEmpty(WorkspaceSearchTerm)
            ? filteredCards.Take(5)
            : filteredCards.Take(20));

        DisplayedWorkspaceCards = _newDisplayCards;
        IsLoading = false;
    }

    public Home_WorkspacesViewModel()
    {
        AddWorkspaceCommand = new RelayCommand(OnAddWorkspace);
    }

    private async void OnAddWorkspace()
    {
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
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        WorkspaceCards.Add(new WorkspaceItem(new WorkspaceModel()
                        {
                            Name = Path.GetFileName(subDirectory),
                            IsFavorite = false,
                            FullWorkspacePath = Path.GetFullPath(subDirectory),
                            IsVisible = false
                        }));
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting workspaces: {ex.Message}");
        }
    }


    public async void HandleTabClick()
    {
        IsLoading = true;

        if (WorkspaceCards.Count == 0)
        {
            await Task.Run(GetWorkspacesFromDirectoryAsync);
        }

        DisplayedWorkspaceCards.Clear();
        DisplayedWorkspaceCards.AddRange(WorkspaceCards.Take(5));
        IsLoading = false;
    }
}