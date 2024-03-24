using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

public class Home_WorkspacesViewModel : ViewModelBase
{
    private ObservableCollection<WorkspaceItem> _workspaceCards;

    public ObservableCollection<WorkspaceItem> WorkspaceCards
    {
        get => _workspaceCards;
        set
        {
            _workspaceCards = value;
            OnPropertyChanged(nameof(WorkspaceCards));
            UpdateCardVisibility();
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
                UpdateCardVisibility();
            }
        }
    }

    public Home_WorkspacesViewModel()
    {
        WorkspaceCards = new ObservableCollection<WorkspaceItem>();
    }

    private void UpdateCardVisibility()
    {
        foreach (var workspace in WorkspaceCards)
        {
            var workspaceSearchTermLower = WorkspaceSearchTerm.ToLower();
            // Converts workspace name to lowercase for comparison search
            var workspaceNameLower = workspace.Name.ToLower();

            if (string.IsNullOrWhiteSpace(workspaceSearchTermLower) ||
                workspaceNameLower.Contains(workspaceSearchTermLower))
            {
                workspace.IsVisible = true;
            }
            else
            {
                workspace.IsVisible = false;
            }
        }

        OnPropertyChanged(nameof(WorkspaceCards));
    }

    private void GetWorkspacesFromDirectory()
    {
        try
        {
            // Get the directory where the application is running
            var currentDirectory = Settings.Default.WorkspacePath;
            var rootDirectory = Path.Combine(currentDirectory, Constants.TopLevelDirectory);
            if (Directory.Exists(rootDirectory))
            {
                var subDirectories = Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories);

                foreach (var subDirectory in subDirectories)
                {
                    WorkspaceCards.Add(new WorkspaceItem(new WorkspaceModel()
                    {
                        Name = Path.GetFileName(subDirectory),
                        IsFavorite = false,
                        FullWorkspacePath = Path.GetFullPath(subDirectory)
                    }));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting workspaces: {ex.Message}");
        }
    }

    private void ResetWorkspaces()
    {
        WorkspaceCards.Clear();
        GetWorkspacesFromDirectory();
    }

    public void HandleTabClick()
    {
        Console.WriteLine("Tab Workspace Clicked!!!!");
        ResetWorkspaces();
    }
}