using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Models;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;
using dev_flow.Views;
using Microsoft.Web.WebView2.WinForms;
using WebView2 = Microsoft.Web.WebView2.Wpf.WebView2;

namespace dev_flow.ViewModels;

public class CodeSandboxPageViewModel : ViewModelBase
{
    private CodeSandboxPageView _codeSandboxPageView;
    public List<LanguageItem?> SupportedLanguagesItemSource { get; set; }

    private LanguageItem? _selectedLanguage;

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

    public CodeSandboxPageViewModel(CodeSandboxPageView codeSandboxPageView)
    {
        _codeSandboxPageView = codeSandboxPageView;

        SupportedLanguagesItemSource = LanguageInfo.Languages;

        SelectedLanguage = SupportedLanguagesItemSource.Find(supportedLanguage =>
            supportedLanguage?.Name == Settings.Default.CodeEditorLanguage);
    }

    private void OnLanguageSelectionChanged()
    {
        var codeEditorWebView = _codeSandboxPageView.FindName("CodeEditorWebView") as WebView2;
        
        if (SelectedLanguage != null && codeEditorWebView?.CoreWebView2 != null)
        {
            codeEditorWebView.CoreWebView2.ExecuteScriptAsync(
                $"monaco.editor.setModelLanguage(editor.getModel(), '{SelectedLanguage?.Name}');");
        }

        Settings.Default.CodeEditorLanguage = SelectedLanguage?.Name;
        Settings.Default.Save();
    }
}