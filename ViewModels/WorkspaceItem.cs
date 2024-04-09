using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Helpers;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

/// <summary>
/// Workspace item base class.
/// </summary>
public class WorkspaceItem : ViewModelBase
{
    private readonly WorkspaceModel _cardModel;
    private readonly WeakReference _parentViewModelReference;
    public static event EventHandler<WorkspaceItemEventArgs>? WorkspaceItemSelected;

    // Properties

    public string Name
    {
        get => _cardModel.Name;
        set
        {
            _cardModel.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public bool IsFavourite
    {
        get => _cardModel.IsFavourite;
        set
        {
            _cardModel.IsFavourite = value;
            OnPropertyChanged(nameof(IsFavourite));
        }
    }

    public bool IsVisible
    {
        get => _cardModel.IsVisible;
        set
        {
            if (_cardModel.IsVisible != value)
            {
                _cardModel.IsVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    public string FullWorkspacePath
    {
        get => _cardModel.FullWorkspacePath;
        set
        {
            if (_cardModel.FullWorkspacePath != value)
            {
                _cardModel.FullWorkspacePath = value;
                OnPropertyChanged(nameof(FullWorkspacePath));
            }
        }
    }

    public string TrimmedBaseWorkspacePath => Constants.TopLevelDirectory + "/" + _cardModel.Name;
    public string TrimmedDocumentsWorkspacePath => "Documents";
    public string LowerCaseName => _cardModel.Name.ToLower();

    public int ID
    {
        get => _cardModel.ID;
        set
        {
            if (_cardModel.ID != value)
            {
                _cardModel.ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }
    }

    public DateTime DateModified
    {
        get => _cardModel.DateModified;
        set
        {
            if (_cardModel.DateModified != value)
            {
                _cardModel.DateModified = value;
                OnPropertyChanged(nameof(DateModified));
            }
        }
    }

    // Commands

    public ICommand EditNameCommand { get; }
    public ICommand ToggleFavouriteCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand WorkspaceEnteredCommand { get; }
    public ICommand ExportWorkspaceCommand { get; }

    public WorkspaceItem(WorkspaceModel model, object parentViewModel)
    {
        _cardModel = model;
        // Use a weak reference to avoid memory leaks
        _parentViewModelReference = new WeakReference(parentViewModel);
        EditNameCommand = new RelayCommand(EditName);
        ToggleFavouriteCommand = new RelayCommand(ToggleFavourite);
        DeleteCommand = new RelayCommand(Delete);
        WorkspaceEnteredCommand = new RelayCommand(OnWorkspaceItemSelected);
        ExportWorkspaceCommand = new AsyncRelayCommand(async () => await ExportWorkspace());
    }

    /// <summary>
    /// Asynchronously exports the singular workspace.
    /// </summary>
    private async Task ExportWorkspace()
    {
        if (_parentViewModelReference.Target is BaseHomeWorkspacesViewModel parentWorkspaceViewModel)
        {
            // Call the parent view model to handle the export
            await parentWorkspaceViewModel.OnExportWorkspaceRun(this);
        }
    }

    /// <summary>
    /// Edits the name of the workspace.
    /// </summary>
    private void EditName()
    {
        if (_parentViewModelReference.Target is BaseHomeWorkspacesViewModel parentWorkspaceViewModel)
        {
            // Call the parent view model to handle the edit
            parentWorkspaceViewModel.EditWorkspaceName(this);
        }
    }

    /// <summary>
    /// Toggles the favourite status of the workspace.
    /// </summary>
    private void ToggleFavourite()
    {
        if (_parentViewModelReference.Target is BaseHomeWorkspacesViewModel parentWorkspaceViewModel)
        {
            // Call the parent view model to handle the toggle
            parentWorkspaceViewModel.FavouriteToggled(this);
        }
    }

    /// <summary>
    /// Deletes the workspace.
    /// </summary>
    private void Delete()
    {
        if (_parentViewModelReference.Target is BaseHomeWorkspacesViewModel parentWorkspaceViewModel)
        {
            // Call the parent view model to handle the delete
            parentWorkspaceViewModel.DeleteWorkspace(this);
        }
    }

    /// <summary>
    /// Updates the modification date of the workspace to the current date and time.
    /// </summary>
    public void UpdateWorkspaceDateModified()
    {
        DateModified = DateTime.Now;
    }

    /// <summary>
    /// Invokes the WorkspaceItemSelected event.
    /// </summary>
    protected virtual void OnWorkspaceItemSelected()
    {
        WorkspaceItemSelected?.Invoke(this, new WorkspaceItemEventArgs(this));
    }
}