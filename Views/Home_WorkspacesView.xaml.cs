using System.Windows;
using System.Windows.Controls;
using dev_flow.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the Home_WorkspacesView.xaml file.
/// </summary>
public partial class Home_WorkspacesView
{
    public Home_WorkspacesView()
    {
        InitializeComponent();
    }

    private void Home_WorkspacesView_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Register the dialog coordinator on load
        DialogParticipation.SetRegister(this, DataContext);
    }

    private void Home_WorkspacesView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Unregister the dialog coordinator as memory leak prevention on unload
        DialogParticipation.SetRegister(this, null);
    }
}