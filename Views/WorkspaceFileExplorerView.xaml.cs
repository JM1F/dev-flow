using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using dev_flow.Managers.Navigation;
using dev_flow.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the WorkspaceFileExplorerView.xaml file.
/// </summary>
public partial class WorkspaceFileExplorerView : IDisposable
{
    // Register the DeleteButtonClicked event
    public static readonly RoutedEvent DeleteButtonClickedEvent =
        EventManager.RegisterRoutedEvent(nameof(DeleteButtonClicked), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WorkspaceFileExplorerView));

    public WorkspaceFileExplorerView()
    {
        InitializeComponent();
        // Subscribe to the Navigated event of the NavigationManager
        NavigationManager.Navigated += NavigationManager_Navigated;
    }

    /// <summary>
    /// The DeleteButtonClicked event is triggered when the delete button is clicked.
    /// </summary>
    public event RoutedEventHandler DeleteButtonClicked
    {
        add => AddHandler(DeleteButtonClickedEvent, value);
        remove => RemoveHandler(DeleteButtonClickedEvent, value);
    }

    /// <summary>
    /// This method raises the DeleteButtonClicked event.
    /// </summary>
    private void OnDeleteButtonClicked()
    {
        RaiseEvent(new RoutedEventArgs(DeleteButtonClickedEvent));
    }

    /// <summary>
    /// This method handles the Navigated event of the NavigationManager.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
    private void NavigationManager_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.ExtraData is WorkspaceItem workspaceItem)
        {
            var workspaceFileExplorerViewModel = new WorkspaceFileExplorerViewModel(workspaceItem);
            // Execute the OpenDirectoryCommand in view model
            workspaceFileExplorerViewModel.OpenDirectoryCommand.Execute(null);
            DataContext = workspaceFileExplorerViewModel;
        }
    }

    private void WorkspaceFileExplorerView_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Register the dialog coordinator on load
        DialogParticipation.SetRegister(this, DataContext);
    }

    private void WorkspaceFileExplorerView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Unregister the dialog coordinator as memory leak prevention on unload
        DialogParticipation.SetRegister(this, null);
    }

    /// <summary>
    /// Called when the delete button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        OnDeleteButtonClicked();
    }

    /// <summary>
    /// Dispose of the resources used.
    /// </summary>
    public void Dispose()
    {
        NavigationManager.Navigated -= NavigationManager_Navigated;
    }
}