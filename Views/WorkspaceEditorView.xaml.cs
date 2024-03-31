using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using dev_flow.Managers.Navigation;
using dev_flow.ViewModels;
using Markdig;
using Microsoft.Web.WebView2.Core;

namespace dev_flow.Views;

public partial class WorkspaceEditorView : IDisposable
{
    public WorkspaceEditorView()
    {
        InitializeComponent();
        InitialiseWebView2Async();

        NavigationManager.Navigated += NavigationManager_Navigated;
    }

    private void NavigationManager_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.ExtraData is WorkspaceItem workspaceItem)
        {
            var workspaceEditorViewModel = new WorkspaceEditorViewModel(workspaceItem, this);
            DataContext = workspaceEditorViewModel;
        }
    }

    private async void InitialiseWebView2Async()
    {
        await MarkdownViewer.EnsureCoreWebView2Async();
    }

    public void Dispose()
    {
        NavigationManager.Navigated -= NavigationManager_Navigated;
    }
}