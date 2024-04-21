using System;
using System.Windows.Input;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Enums;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;
using MaterialDesignThemes.Wpf;

namespace dev_flow.ViewModels;

public class AddTasksDialogViewModel : ViewModelBase
{
    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    private string _description;

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    private KanbanSeverityEnum _severity;

    public KanbanSeverityEnum Severity
    {
        get => _severity;
        set
        {
            _severity = value;
            OnPropertyChanged(nameof(Severity));
        }
    }

    private DateTime? _dueDate;

    public DateTime? DueDate
    {
        get => _dueDate;
        set
        {
            _dueDate = value;
            OnPropertyChanged(nameof(DateTime));
        }
    }

    public ICommand SaveTaskCommand { get; set; }

    private readonly KanbanStatusEnum _kanbanStatusEnum;

    public AddTasksDialogViewModel(KanbanStatusEnum kanbanStatusEnum)
    {
        _kanbanStatusEnum = kanbanStatusEnum;
        SaveTaskCommand = new RelayCommand(SaveTask);
    }

    private void SaveTask()
    {
        DialogHost.CloseDialogCommand.Execute(new KanbanTask
        {
            ID = Guid.NewGuid(),
            Title = Title,
            Description = Description,
            Status = _kanbanStatusEnum,
            Severity = Severity,
            DueDate = DueDate ?? DateTime.Now
        }, null);
    }
}