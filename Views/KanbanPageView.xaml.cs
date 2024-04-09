using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dev_flow.Models;
using dev_flow.ViewModels;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the KanbanPageView.xaml file.
/// </summary>
public partial class KanbanPageView : UserControl
{
    public KanbanPageView()
    {
        InitializeComponent();
        DataContext = new KanbanPageViewModel();
    }
}