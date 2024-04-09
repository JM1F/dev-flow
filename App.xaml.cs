using System;
using System.IO;
using System.Windows;
using ControlzEx.Theming;
using dev_flow.Enums;
using dev_flow.Properties;
using Constants = dev_flow.Interfaces.Constants;

namespace dev_flow
{
    /// <summary>
    /// Code-behind for the App.xaml file.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Called when the application is started. This method is the entry point of the application.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CreateWorkspaceDirectory();
            SetTheme();
        }

        /// <summary>
        /// Creates the workspace directory if it does not exist.
        /// </summary>
        private void CreateWorkspaceDirectory()
        {
            // Get the directory where the application is running
            var currentDirectory = Settings.Default.WorkspacePath;

            // Combine the app directory path with the directory name
            var directoryPath = Path.Combine(currentDirectory, Constants.TopLevelDirectory);

            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    // If not, create it
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine("Base directory created successfully.");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that might occur during directory creation
                    Console.WriteLine($"Error creating base directory: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Base directory already exists.");
            }
        }

        /// <summary>
        /// Sets the theme of the application based on the user's settings.
        /// </summary>
        private void SetTheme()
        {
            ThemeManager.Current.ChangeTheme(this,
                Settings.Default.Theme == ThemeEnum.LightTheme ? "Light.Steel" : "Dark.Steel");
        }
    }
}