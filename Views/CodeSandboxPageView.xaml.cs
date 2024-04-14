using System;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dev_flow.Enums;
using dev_flow.Properties;
using dev_flow.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Web.WebView2.Core;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the CodeSandboxPageView.xaml file.
/// </summary>
public partial class CodeSandboxPageView : UserControl
{
    private const string EditorFileName = @"Monaco\codeEditor.html";
    private const string EditorTemplateFileName = @"Monaco\codeEditorTemplate.html";

    private string _editorHtmlPath;
    private string _editorHtmlContent;

    public CodeSandboxPageView()
    {
        InitializeComponent();
        DataContext = new CodeSandboxPageViewModel(this);

        _editorHtmlPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, EditorFileName);

        InitialiseEditorAsync();
    }

    private async Task LoadEditorHtml(string theme, string? language = null)
    {
        _editorHtmlContent = await File.ReadAllTextAsync(_editorHtmlPath);

        var templatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, EditorTemplateFileName);
        
        var codeEditorValue = Settings.Default.CodeEditorValue;
        string htmlContent = await File.ReadAllTextAsync(templatePath);
        htmlContent = htmlContent.Replace("{{THEME}}", theme);
        htmlContent = htmlContent.Replace("{{LANGUAGE}}", Settings.Default.CodeEditorLanguage);
        htmlContent = htmlContent.Replace("{{EDITOR_VALUE}}",codeEditorValue);

        await File.WriteAllTextAsync(_editorHtmlPath, htmlContent);

        CodeEditorWebView.Source = new Uri(_editorHtmlPath);
    }

    private async void InitialiseEditorAsync()
    {
        await CodeEditorWebView.EnsureCoreWebView2Async();

        if (Settings.Default.Theme == ThemeEnum.DarkTheme)
        {
            await LoadEditorHtml("vs-dark");
        }
        else
        {
            await LoadEditorHtml("vs");
        }
    }

    private void CodeEditorWebView_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            e.Handled = true;
        }
    }

    private void CodeEditorWebView_OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (CodeEditorWebView != null)
        {
            //CodeEditorWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            CodeEditorWebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsWebMessageEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            //CodeEditorWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
        }
    }

    private void CodeEditorWebView_OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        CodeEditorWebView.Visibility = Visibility.Visible;
    }
}