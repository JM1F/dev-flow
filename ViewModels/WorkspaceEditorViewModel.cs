using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx.Theming;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.ViewModels.Shared;
using Markdig;
using Microsoft.Web.WebView2.Wpf;

namespace dev_flow.ViewModels;

/// <summary>
/// View model for the workspace editor.
/// </summary>
public class WorkspaceEditorViewModel : ViewModelBase
{
    private bool _isPreviewMode;
    private readonly UserControl _view;
    private WorkspaceItem _workspaceItem;

    // Properties

    private string _markdownContent;

    public string MarkdownContent
    {
        get => _markdownContent;
        set
        {
            _markdownContent = value;
            OnPropertyChanged(nameof(MarkdownContent));
            // Call asynchronous method to render markdown content
            _ = RenderMarkdown();
        }
    }

    public string WorkspaceTitleName => _workspaceItem.Name;
    public bool WorkspaceIsFavourite => _workspaceItem.IsFavourite;


    private string RenderedHtml { get; set; }

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

    // Commands
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand PreviewCommand { get; }


    public WorkspaceEditorViewModel(WorkspaceItem workspaceItem, UserControl view)
    {
        _view = view;
        _workspaceItem = workspaceItem;

        EditCommand = new RelayCommand(() => IsPreviewMode = false);
        SaveCommand = new RelayCommand(SaveMarkdown);
        PreviewCommand = new RelayCommand(() => IsPreviewMode = true);

        // Call asynchronous method to load markdown content
        _ = LoadMarkdownContent();
    }


    /// <summary>
    /// Asynchronously loads the markdown content from the file.
    /// </summary>
    private async Task LoadMarkdownContent()
    {
        try
        {
            var markdownFilePath = Path.GetFullPath(_workspaceItem.TrimmedBaseWorkspacePath + "/markdown.md");
            MarkdownContent = await File.ReadAllTextAsync(markdownFilePath);
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            Console.WriteLine($@"Error loading markdown content: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously renders the markdown content to HTML.
    /// </summary>
    private async Task RenderMarkdown()
    {
        try
        {
            // Create Markdig pipeline
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            // Asynchronously render markdown content to HTML via Markdig pipeline
            var workspaceMarkdownHtml = await Task.Run(() => Markdown.ToHtml(_markdownContent, pipeline));

            var currentThemeName = ThemeManager.Current.DetectTheme()?.Name;
            if (currentThemeName != null)
            {
                // Get the HTML background and foreground colours based on the current application theme
                var htmlBackgroundColour = GetWebBackgroundColourFromTheme(currentThemeName);
                var htmlForegroundColour = GetWebForegroundColourFromTheme(currentThemeName);

                // Create embedded HTML content
                RenderedHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>{_workspaceItem.Name}</title>
                    <style>
                        body {{
                            background-color: {htmlBackgroundColour};
                            color: {htmlForegroundColour};
                            font-family: 'Segoe UI', sans-serif;
                        }}
                        pre {{
                            background-color: #939393;
                            padding: 10px;
                        }}

                        code {{
                            font-family: 'Consolas', monospace;
                            font-size: 14px;
                        }}
                    </style>
                </head>
                <body>
                    {workspaceMarkdownHtml}
                </body>
                </html>";
            }

            TogglePreviewMode();
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            // Log error rendering exception
            Console.WriteLine($@"Error rendering markdown content: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the web foreground colour from the theme.
    /// </summary>
    /// <param name="currentThemeName">The current theme name.</param>
    /// <returns>The HTML colour code for the foreground colour.</returns>
    private string GetWebForegroundColourFromTheme(string currentThemeName)
    {
        Color htmlForegroundColour;
        if (currentThemeName == "Light.Steel")
        {
            htmlForegroundColour = Color.FromArgb(0, 0, 0);
        }
        else
        {
            htmlForegroundColour = Color.FromArgb(255, 255, 255);
        }

        return ColorTranslator.ToHtml(htmlForegroundColour);
    }

    /// <summary>
    /// Gets the web background colour from the theme.
    /// </summary>
    /// <param name="currentThemeName">The current theme name.</param>
    /// <returns>The HTML colour code for the background colour.</returns>
    private string GetWebBackgroundColourFromTheme(string currentThemeName)
    {
        Color htmlBackgroundColour;
        if (currentThemeName == "Light.Steel")
        {
            htmlBackgroundColour = Color.FromArgb(224, 228, 231);
        }
        else
        {
            htmlBackgroundColour = Color.FromArgb(50, 54, 57);
        }

        return ColorTranslator.ToHtml(htmlBackgroundColour);
    }

    /// <summary>
    /// Toggles the preview mode of the markdown editor and viewer.
    /// </summary>
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
                // Load the rendered HTML content into the WebView2 control
                markdownViewer.CoreWebView2.NavigateToString(RenderedHtml);
            }
        }
    }

    /// <summary>
    /// Asynchronously saves the markdown content to the file.
    /// </summary>
    private async void SaveMarkdown()
    {
        try
        {
            IsPreviewMode = false;

            var markdownFilePath = Path.GetFullPath(_workspaceItem.TrimmedBaseWorkspacePath + "/markdown.md");
            await File.WriteAllTextAsync(markdownFilePath, _markdownContent);

            // Update the workspace date modified
            _workspaceItem.UpdateWorkspaceDateModified();
        }
        // Handle any exceptions
        catch (Exception ex)
        {
            // Log saving markdown error
            Console.WriteLine($"Error saving markdown: {ex.Message}");
        }
    }
}