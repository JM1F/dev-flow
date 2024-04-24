using System;
using System.Windows.Controls;
using dev_flow.ViewModels;

namespace dev_flow.Assets.Styles;

/// <summary>
/// Code behind for the AddTaskDialog user control.
/// </summary>
public partial class AddTaskDialog : UserControl
{
    public AddTaskDialog()
    {
        InitializeComponent();
        // Set the date picker to the current date.
        AddTaskDatePicker.SelectedDate = DateTime.Now;
        AddTaskDatePicker.DisplayDate = DateTime.Now;
    }
}