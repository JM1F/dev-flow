using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the CodeSandboxPageView.xaml file.
/// </summary>
public partial class CodeSandboxPageView : UserControl
{
    public CodeSandboxPageView()
    {
        InitializeComponent();
    }

    private void CodeSandboxPageView_OnLoaded(object sender, RoutedEventArgs e)
    {
        CodeEditorWebView.Source =
            new Uri(System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"Monaco\index.html"));
    }
}