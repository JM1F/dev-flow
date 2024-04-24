using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using dev_flow.Enums;

namespace dev_flow.Models;

/// <summary>
/// Represents a task in a Kanban board.
/// </summary>
public class KanbanTask : INotifyPropertyChanged
{
    private Guid _id;
    private string _title;
    private string _description;
    private KanbanSeverityEnum _severity;
    private DateTime _dueDate;
    private DateTime _dateCreated;
    private KanbanStatusEnum _status;

    [XmlElement("ID")]
    public Guid ID
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged(nameof(ID));
        }
    }
    
    [XmlElement("Title")]
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    [XmlElement("Description")]
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    [XmlElement("Severity")]
    public KanbanSeverityEnum Severity
    {
        get => _severity;
        set
        {
            _severity = value;
            OnPropertyChanged(nameof(Severity));
        }
    }

    [XmlElement("DueDate")]
    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            _dueDate = value;
            OnPropertyChanged(nameof(DueDate));
        }
    }

    [XmlElement("DateCreated")]
    public DateTime DateCreated
    {
        get => _dateCreated;
        set
        {
            _dateCreated = value;
            OnPropertyChanged(nameof(DateCreated));
        }
    }

    [XmlElement("Status")]
    public KanbanStatusEnum Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}