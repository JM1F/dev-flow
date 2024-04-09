using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

/// <summary>
/// View model for the About page.
/// </summary>
public class AboutPageViewModel : ViewModelBase
{
    public ICommand OpenGitHubRepoCommand { get; }
    public ICommand OpenGitHubIssuesCommand { get; }

    public AboutPageViewModel()
    {
        OpenGitHubRepoCommand = new RelayCommand(OnOpenGitHubRepo);
        OpenGitHubIssuesCommand = new RelayCommand(OnOpenGitHubIssues);
    }

    // Opens the GitHub issues page in the default browser.
    private void OnOpenGitHubIssues()
    {
        try
        {
            var url = "https://github.com/JM1F/dev-flow/issues";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            // Handle the exception and show a error message box
            MessageBox.Show($"Unable to open the repository issues link: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // Opens the GitHub repository in the default browser.
    private void OnOpenGitHubRepo()
    {
        try
        {
            var url = "https://github.com/JM1F/dev-flow";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            // Handle the exception and show a error message box
            MessageBox.Show($"Unable to open the repository link: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}