using System;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using dev_flow.Constants;
using dev_flow.Enums;
using dev_flow.Properties;
using dev_flow.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Web.WebView2.Core;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the CodeSandboxPageView.xaml file.
/// </summary>
public partial class CodeSandboxPageView : UserControl
{
    private string _editorHtmlPath;

    public CodeSandboxPageView()
    {
        InitializeComponent();
        DataContext = new CodeSandboxPageViewModel(this);


        _editorHtmlPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, DevFlowConstants.CodeEditorFileName);

        _ = InitialiseEditorAsync();
    }

    /// <summary>
    /// Loads the editor HTML content.
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="language"></param>
    private async Task LoadEditorHtml(string theme, string? language = null)
    {
        var templatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, DevFlowConstants.CodeEditorTemplateFileName);

        var htmlContent = await File.ReadAllTextAsync(templatePath);

        htmlContent = htmlContent.Replace("{{THEME}}", theme);
        htmlContent = htmlContent.Replace("{{LANGUAGE}}", Settings.Default.CodeEditorLanguage);

        await File.WriteAllTextAsync(_editorHtmlPath, htmlContent);

        CodeEditorWebView.Source = new Uri(_editorHtmlPath);
    }

    /// <summary>
    /// Initialises the code editor.
    /// </summary>
    private async Task InitialiseEditorAsync()
    {
        try
        {
            // Ensure the CoreWebView2 is ready
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
        catch (COMException ex) when (ex.HResult == unchecked((int)0x80004004))
        {
            // Handle the specific COMException
            Console.WriteLine($"WebView2 initialisation aborted: {ex.Message}");

            // Dispose of the WebView2 control
            CodeEditorWebView?.Dispose();
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            var errorMessage = $"WebView2 initialisation error: {ex.Message}";

            Console.WriteLine($"WebView2 initialisation error: {ex.Message}");
            MessageBox.Show(errorMessage, "Initialisation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Dispose of the WebView2 control
            CodeEditorWebView?.Dispose();
        }
    }

    /// <summary>
    /// Handles the KeyDown event of the CodeEditorWebView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CodeEditorWebView_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Handles the NavigationStarting event of the CodeEditorWebView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CodeEditorWebView_OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (CodeEditorWebView != null)
        {
            CodeEditorWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            CodeEditorWebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsWebMessageEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            CodeEditorWebView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
        }
    }

    /// <summary>
    /// Handles the NavigationCompleted event of the CodeEditorWebView control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void CodeEditorWebView_OnNavigationCompleted(object? sender,
        CoreWebView2NavigationCompletedEventArgs e)
    {
        // Set the code editor value in webview2 model
        await CodeEditorWebView.CoreWebView2.ExecuteScriptAsync(
            $"monaco.editor.getModels()[0].setValue(`{Settings.Default.CodeEditorValue}`);");

        CodeEditorProgressRing.Visibility = Visibility.Collapsed;
        CodeEditorWebView.Visibility = Visibility.Visible;
    }

    private void CodeSandboxPageView_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Register the dialog coordinator on load
        DialogParticipation.SetRegister(this, DataContext);
    }

    private void CodeSandboxPageView_OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Unregister the dialog coordinator as memory leak prevention on unload
        DialogParticipation.SetRegister(this, null);
    }
}