using System;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Interfaces;
using dev_flow.Models;
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

    public WorkspaceItem(WorkspaceModel model, object parentViewModel)
    {
        _cardModel = model;
        _parentViewModelReference = new WeakReference(parentViewModel);
        EditNameCommand = new RelayCommand(EditName);
        ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);
        DeleteCommand = new RelayCommand(Delete);
    }

    private void EditName()
    {
        if (_parentViewModelReference.Target is Home_WorkspacesViewModel parentWorkspaceViewModel)
        {
            parentWorkspaceViewModel.EditWorkspaceName(this);
        }
        else if (_parentViewModelReference.Target is Home_FavouritesViewModel parentFavouriteViewModel)
        {
            parentFavouriteViewModel.EditWorkspaceName(this);
        }
    }

    private void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
    }

    private void Delete()
    {
        if (_parentViewModelReference.Target is Home_WorkspacesViewModel parentWorkspaceViewModel)
        {
            parentWorkspaceViewModel.DeleteWorkspace(this);
        }
        else if (_parentViewModelReference.Target is Home_FavouritesViewModel parentFavouriteViewModel)
        {
            parentFavouriteViewModel.DeleteWorkspace(this);
        }
    }
}