using System;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Helpers;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

public class WorkspaceItem : ViewModelBase
{
    private readonly WorkspaceModel _cardModel;
    private readonly WeakReference _parentViewModelReference;

    public string Name
    {
        get => _cardModel.Name;
        set
        {
            _cardModel.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public bool IsFavorite
    {
        get => _cardModel.IsFavorite;
        set
        {
            _cardModel.IsFavorite = value;
            OnPropertyChanged(nameof(IsFavorite));
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

    public string TrimmedWorkspacePath => Constants.TopLevelDirectory + "/" + _cardModel.Name;
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

    public ICommand EditNameCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand WorkspaceEnteredCommand { get; }

    public static event EventHandler<WorkspaceItemEventArgs> WorkspaceItemSelected;

    public WorkspaceItem(WorkspaceModel model, object parentViewModel)
    {
        _cardModel = model;
        _parentViewModelReference = new WeakReference(parentViewModel);
        EditNameCommand = new RelayCommand(EditName);
        ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);
        DeleteCommand = new RelayCommand(Delete);
        WorkspaceEnteredCommand = new RelayCommand(OnWorkspaceItemSelected);
    }

    private void EditName()
    {
        if (_parentViewModelReference.Target is Home_WorkspacesViewModel parentWorkspaceViewModel)
        {
            parentWorkspaceViewModel.EditWorkspaceName(this);
        }
    }

    private void ToggleFavorite()
    {
        if (_parentViewModelReference.Target is Home_WorkspacesViewModel parentWorkspaceViewModel)
        {
            parentWorkspaceViewModel.FavouriteToggled(this);
        }
    }

    private void Delete()
    {
        if (_parentViewModelReference.Target is Home_WorkspacesViewModel parentWorkspaceViewModel)
        {
            parentWorkspaceViewModel.DeleteWorkspace(this);
        }
    }

    protected virtual void OnWorkspaceItemSelected()
    {
        WorkspaceItemSelected?.Invoke(this, new WorkspaceItemEventArgs(this));
    }
}