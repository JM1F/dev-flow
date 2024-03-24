using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

public class WorkspaceItem : ViewModelBase
{
    private readonly WorkspaceModel _cardModel;
    
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

    public ICommand EditNameCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand DeleteCommand { get; }

    public WorkspaceItem(WorkspaceModel model)
    {
        _cardModel = model;
        EditNameCommand = new RelayCommand(EditName);
        ToggleFavoriteCommand = new RelayCommand(ToggleFavorite);
        DeleteCommand = new RelayCommand(Delete);
        
        IsVisible = true;
    }

    private void EditName()
    {
        // Implement logic for editing the name
    }

    private void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
    }

    private void Delete()
    {
        // Implement logic for deleting the card
    }
}