using System;
using System.Windows.Controls;
using dev_flow.ViewModels;

namespace dev_flow.Assets.Styles;

public partial class AddTaskDialog : UserControl
{
    public AddTaskDialog()
    {
        InitializeComponent();
        AddTaskDatePicker.SelectedDate = DateTime.Now;
        AddTaskDatePicker.DisplayDate = DateTime.Now;
    }
}