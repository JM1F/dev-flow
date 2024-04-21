using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Constants;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using dev_flow.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace dev_flow.ViewModels;

public class CodeSandboxPageViewModel : ViewModelBase
{
    private CodeSandboxPageView _codeSandboxPageView;
    private WebView2? _codeEditorWebView;
    private string _codeEditorValue;
    public List<LanguageItem?> SupportedLanguagesItemSource { get; set; }

    private LanguageItem? _selectedLanguage;
    private readonly DialogCoordinator _dialogCoordinator;

    public LanguageItem? SelectedLanguage
    {
        get { return _selectedLanguage; }
        set
        {
            _selectedLanguage = value;
            OnPropertyChanged(nameof(SelectedLanguage));
            OnLanguageSelectionChanged();
        }
    }

    public ICommand SaveCodeLocallyCommand { get; set; }
    public ICommand SaveCodeAsCommand { get; set; }
    public ICommand OpenCodeCommand { get; set; }

    public CodeSandboxPageViewModel(CodeSandboxPageView codeSandboxPageView)
    {
        _codeSandboxPageView = codeSandboxPageView;

        _codeEditorWebView = codeSandboxPageView.FindName("CodeEditorWebView") as WebView2;

        SupportedLanguagesItemSource = LanguageInfo.Languages;

        SelectedLanguage = SupportedLanguagesItemSource.Find(supportedLanguage =>
            supportedLanguage?.Name == Settings.Default.CodeEditorLanguage);

        _dialogCoordinator = new DialogCoordinator();

        SaveCodeLocallyCommand = new AsyncRelayCommand(SaveCodeLocally);
        SaveCodeAsCommand = new AsyncRelayCommand(SaveCodeAs);
        OpenCodeCommand = new AsyncRelayCommand(OpenCode);
    }

    private async Task OpenCode()
    {
        // Generate the filter string dynamically based on the LanguageInfo class and the attributes
        var filterString = string.Join("|", LanguageInfo.Languages
            .Where(lang => lang != null)
            .Select(lang => $"{lang?.DisplayName} Files (*{lang?.Extension})|*{lang?.Extension}"));

        // Append the "All Files" filter at the end
        filterString += "|All Files (*.*)|*.*";

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = filterString
        };

        if (openFileDialog.ShowDialog() == true && _codeEditorWebView != null)
        {
            _codeEditorWebView.Visibility = Visibility.Collapsed;

            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this,
                "Opening Code", "Please wait while the code is being opened...");
            progressDialogController.SetIndeterminate();

            var fileName = openFileDialog.FileName;
            var fileContent = await File.ReadAllTextAsync(fileName);

            await _codeEditorWebView.CoreWebView2.ExecuteScriptAsync(
                $"monaco.editor.getModels()[0].setValue(`{fileContent}`);");

            await progressDialogController.CloseAsync();

            _codeEditorWebView.Visibility = Visibility.Visible;
        }
    }

    private async Task SaveCodeAs()
    {
        if (_codeEditorWebView != null)
        {
            _codeEditorWebView.Visibility = Visibility.Collapsed;

            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this,
                "Saving Code", "Please wait while the code is being saved...");

            progressDialogController.SetIndeterminate();

            await GetAndStripCodeFromEditor();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"DevFlow_{SelectedLanguage?.DisplayName}", // Default file name
                DefaultExt = SelectedLanguage?.Extension, // Default file extension based on selected language
                Filter = "All Files (*.*)|*.*" // Allow any file extension
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                await File.WriteAllTextAsync(filePath, _codeEditorValue);

                await SaveCodeToSettings(_codeEditorValue);
            }

            await progressDialogController.CloseAsync();

            _codeEditorWebView.Visibility = Visibility.Visible;
        }
    }

    private async Task SaveCodeLocally()
    {
        if (_codeEditorWebView != null)
        {
            _codeEditorWebView.Visibility = Visibility.Collapsed;

            var progressDialogController = await _dialogCoordinator.ShowProgressAsync(this,
                "Saving Code Locally", "Please wait while the code is being saved locally...");

            progressDialogController.SetIndeterminate();

            await GetAndStripCodeFromEditor();

            await SaveCodeToSettings(_codeEditorValue);

            await progressDialogController.CloseAsync();

            _codeEditorWebView.Visibility = Visibility.Visible;
        }
    }

    private async Task GetAndStripCodeFromEditor()
    {
        if (_codeEditorWebView != null)
        {
            var rawCodeEditorValue = await _codeEditorWebView.CoreWebView2.ExecuteScriptAsync("editor.getValue();");
            var editedCodeEditorValue = rawCodeEditorValue;

            if (rawCodeEditorValue.StartsWith("\"") && rawCodeEditorValue.EndsWith("\""))
            {
                // Remove the starting and ending double quotes
                editedCodeEditorValue = rawCodeEditorValue.Substring(1, rawCodeEditorValue.Length - 2);
            }

            // Decode escape sequences with regex, but preserve literal occurrences within the code itself
            editedCodeEditorValue = Regex.Replace(editedCodeEditorValue,
                @"(?<!\\)(\\r\\n|\\n|\\t|\\v|\\f|\\b|\\r|\\x[0-9A-Fa-f]{2}|\\u[0-9A-Fa-f]{4}|\\""|\\/(?!\\/)|\\+(?![rntfvbux0-9/]))",
                match =>
                {
                    switch (match.Value)
                    {
                        case "\\r\\n":
                            return "\r\n";
                        case "\\n":
                            return "\n";
                        case "\\t":
                            return "\t";
                        case "\\v":
                            return "\v";
                        case "\\f":
                            return "\f";
                        case "\\b":
                            return "\b";
                        case "\\r":
                            return "\r";
                        case var hex when hex.StartsWith("\\x"):
                            return ((char)Convert.ToInt32(hex.Substring(2), 16)).ToString();
                        case var unicode when unicode.StartsWith("\\u"):
                            return ((char)Convert.ToInt32(unicode.Substring(2), 16)).ToString();
                        case "\\\"":
                            return "\"";
                        case "\\/":
                            return "/";
                        case "(`)":
                            return "{{BACKTICK}}";
                        default:
                            if (match.Value.StartsWith("\\"))
                            {
                                return new string('\\', match.Value.Length / 2);
                            }

                            return match.Value;
                    }
                },
                RegexOptions.None);

            _codeEditorValue = editedCodeEditorValue;
        }
    }

    private Task SaveCodeToSettings(string? code)
    {
        Settings.Default.CodeEditorValue = code;
        Settings.Default.Save();

        return Task.CompletedTask;
    }

    private void OnLanguageSelectionChanged()
    {
        if (SelectedLanguage != null && _codeEditorWebView?.CoreWebView2 != null)
        {
            _codeEditorWebView.CoreWebView2.ExecuteScriptAsync(
                $"monaco.editor.setModelLanguage(editor.getModel(), `{SelectedLanguage?.Name}`);");
        }

        Settings.Default.CodeEditorLanguage = SelectedLanguage?.Name;
        Settings.Default.Save();
    }
}