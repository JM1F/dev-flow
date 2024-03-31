using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Interfaces;
using dev_flow.ViewModels.Shared;
using dev_flow.Views;
using Markdig;
using Microsoft.Web.WebView2.Wpf;

namespace dev_flow.ViewModels;

public class WorkspaceEditorViewModel : ViewModelBase
{
    private string _markdownContent;
    private bool _isPreviewMode;
    private bool _isEditing;
    private UserControl _view;

    public string MarkdownContent
    {
        get => _markdownContent;
        set
        {
            _markdownContent = value;
            OnPropertyChanged(nameof(MarkdownContent));
            RenderMarkdown();
        }
    }

    public string WorkspaceTitleName => _workspaceItem.Name;

    private WorkspaceItem _workspaceItem;
    public ICommand Test { get; }

    public WorkspaceEditorViewModel(WorkspaceItem workspaceItem, WorkspaceEditorView view)
    {
        _view = view;
        _workspaceItem = workspaceItem;

        Test = new RelayCommand(OnTest);
        EditCommand = new RelayCommand(() => IsPreviewMode = false);
        SaveCommand = new RelayCommand(SaveMarkdown);
        PreviewCommand = new RelayCommand(() => IsPreviewMode = true);

        LoadMarkdownContent();
    }


    private void OnTest()
    {
        Console.WriteLine("NO WAAAAAAAY");
    }

    public string RenderedHtml { get; private set; }

    public bool IsPreviewMode
    {
        get => _isPreviewMode;
        set
        {
            _isPreviewMode = value;
            OnPropertyChanged(nameof(IsPreviewMode));
            TogglePreviewMode();
        }
    }

    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand PreviewCommand { get; }

    private void LoadMarkdownContent()
    {
        _markdownContent = File.ReadAllText(_workspaceItem.TrimmedWorkspacePath + "/" + "markdown.md");
        RenderMarkdown();
    }

    private void RenderMarkdown()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        RenderedHtml = Markdown.ToHtml(_markdownContent, pipeline);

        TogglePreviewMode();
    }

    private void TogglePreviewMode()
    {
        var markdownEditor = _view.FindName("MarkdownEditor") as TextBox;
        var markdownViewer = _view.FindName("MarkdownViewer") as WebView2;

        if (markdownEditor != null && markdownViewer != null)
        {
            markdownEditor.Visibility = IsPreviewMode ? Visibility.Collapsed : Visibility.Visible;
            markdownViewer.Visibility = IsPreviewMode ? Visibility.Visible : Visibility.Collapsed;

            if (IsPreviewMode)
            {
                markdownViewer.CoreWebView2.NavigateToString(RenderedHtml);
            }
        }
    }

    private void SaveMarkdown()
    {
        IsPreviewMode = false;
        File.WriteAllText(_workspaceItem.TrimmedWorkspacePath + "/" + "markdown.md", _markdownContent);
    }
}