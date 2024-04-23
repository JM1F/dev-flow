using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dev_flow.Models;
using dev_flow.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the KanbanPageView.xaml file.
/// </summary>
public partial class KanbanPageView : IDisposable
{
    public KanbanPageView()
    {
        InitializeComponent();
        DataContext = new KanbanPageViewModel();
    }

    private void KanbanPageView_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Register the dialog coordinator on load
        DialogParticipation.SetRegister(this, DataContext);
    }

    private void KanbanPageView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Unregister the dialog coordinator as memory leak prevention on unload
        DialogParticipation.SetRegister(this, null);
    }

    public void Dispose()
    {
        ListBoxTodo.ItemsSource = null;
        ListBoxTodo = null;

        ListBoxDoing.ItemsSource = null;
        ListBoxDoing = null;

        ListBoxDone.ItemsSource = null;
        ListBoxDone = null;
    }
}