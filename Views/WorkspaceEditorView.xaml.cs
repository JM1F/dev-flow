using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ControlzEx.Theming;
using dev_flow.Managers.Navigation;
using dev_flow.ViewModels;
using Markdig;
using Microsoft.Web.WebView2.Core;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the WorkspaceEditorView.xaml file.
/// </summary>
public partial class WorkspaceEditorView : IDisposable
{
    private CoreWebView2? _coreWebView2;
    private WorkspaceEditorViewModel _workspaceEditorViewModel;

    public WorkspaceEditorView()
    {
        InitializeComponent();
        InitialiseWebView2Async();

        // Subscribe to the Navigated event of the NavigationManager
        NavigationManager.Navigated += NavigationManager_Navigated;
    }

    /// <summary>
    /// Handles the Click event of the GoBackButton.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void NavigationManager_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.ExtraData is WorkspaceItem workspaceItem)
        {
            _workspaceEditorViewModel = new WorkspaceEditorViewModel(workspaceItem, this);
            DataContext = _workspaceEditorViewModel;
        }
    }

    /// <summary>
    /// Initialises the WebView2 control asynchronously.
    /// </summary>
    private async void InitialiseWebView2Async()
    {
        // Ensure that the CoreWebView2 is ready
        await MarkdownViewer.EnsureCoreWebView2Async();
        _coreWebView2 = MarkdownViewer.CoreWebView2;

        // Subscribe to the NewWindowRequested and NavigationStarting events of the CoreWebView2 control
        _coreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        _coreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

        _coreWebView2.ContextMenuRequested += WebView_ContextMenuRequested;

        _coreWebView2.Settings.AreDevToolsEnabled = false;
        _coreWebView2.Settings.IsStatusBarEnabled = false;
        _coreWebView2.Settings.IsSwipeNavigationEnabled = false;
        _coreWebView2.Settings.AreHostObjectsAllowed = false;
        _coreWebView2.Settings.IsPasswordAutosaveEnabled = false;
        _coreWebView2.Settings.IsWebMessageEnabled = false;
    }

    /// <summary>
    /// Handles the ContextMenuRequested event of the WebView2 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="CoreWebView2ContextMenuRequestedEventArgs"/> instance containing the event data.</param>
    private void WebView_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
    {
        var contextMenuItems = e.MenuItems;

        // Disable the Reload option from the context menu to prevent the user from reloading the page
        var reloadMenuItem = contextMenuItems.FirstOrDefault(item => item.Name == "reload");
        if (reloadMenuItem != null)
        {
            contextMenuItems.Remove(reloadMenuItem);
        }

        // Disable the Forward option from the context menu to prevent the user from navigating forward
        var forwardMenuItem = contextMenuItems.FirstOrDefault(item => item.Name == "forward");
        if (forwardMenuItem != null)
        {
            contextMenuItems.Remove(forwardMenuItem);
        }

        // Remove the Back option to prevent the user from navigating back
        var backMenuItem = contextMenuItems.FirstOrDefault(item => item.Name == "back");
        if (backMenuItem != null)
        {
            contextMenuItems.Remove(backMenuItem);
        }
    }


    /// <summary>
    /// Handles the NavigationStarting event of the WebView2 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="CoreWebView2NavigationStartingEventArgs"/> instance containing the event data.</param>
    private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (IsExternalLink(e.Uri))
        {
            // Cancel the navigation within the WebView2 control
            e.Cancel = true;

            // Open the link in the default web browser
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri,
                UseShellExecute = true
            });
        }
        else if (e.Uri.StartsWith("file://"))
        {
            // Cancel the navigation within the WebView2 control
            e.Cancel = true;

            // Open the file in the default application
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri,
                UseShellExecute = true
            });
        }
    }

    /// <summary>
    /// Determines whether the specified URI is an external link.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns> true if the specified URI is an external link; otherwise, false.
    /// </returns>
    private bool IsExternalLink(string uri)
    {
        return uri.StartsWith("http://") || uri.StartsWith("https://");
    }

    /// <summary>
    /// Handles the NewWindowRequested event of the WebView2 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="CoreWebView2NewWindowRequestedEventArgs"/> instance containing the event data.</param>
    private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        // Prevent the default behavior of opening the link within the WebView2 control
        e.Handled = true;

        // Open the link in the default web browser
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = e.Uri,
            UseShellExecute = true
        });
    }

    /// <summary>
    /// Handles the Unloaded event of the WorkspaceEditorView.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void WorkspaceEditorView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_coreWebView2 != null)
        {
            _coreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
            _coreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
            _coreWebView2.ContextMenuRequested -= WebView_ContextMenuRequested;

            _coreWebView2 = null;
        }
    }

    /// <summary>
    /// Handles the DeleteButtonClicked event of the WorkspaceFileExplorerView.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void WorkspaceFileExplorerView_OnDeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        if (_workspaceEditorViewModel.IsPreviewMode)
        {
            WorkspaceEditorRibbon.EditViewRadioButton.IsChecked = true;
            _workspaceEditorViewModel.EditCommand.Execute(null);
        }
    }

    /// <summary>
    /// Releases all resources used by the WorkspaceEditorView.
    /// </summary>
    public void Dispose()
    {
        NavigationManager.Navigated -= NavigationManager_Navigated;
    }

    private void MarkdownViewer_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            e.Handled = true;
        }
    }
}