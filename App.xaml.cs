using System;
using System.IO;
using System.Windows;
using ControlzEx.Theming;
using dev_flow.Enums;
using dev_flow.Properties;

namespace dev_flow
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CreateWorkspaceDirectory();
            SetTheme();
        }

        private void CreateWorkspaceDirectory()
        {
            // Get the directory where the application is running
            string currentDirectory = Settings.Default.WorkspacePath;

            // Directory name
            string directoryName = "DEV_FLOW_WORKSPACES";

            // Combine the app directory path with the directory name
            string directoryPath = Path.Combine(currentDirectory, directoryName);

            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    // If not, create it
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine("Directory created successfully.");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that might occur during directory creation
                    Console.WriteLine($"Error creating directory: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Directory already exists.");
            }
        }

        private void SetTheme()
        {
            ThemeManager.Current.ChangeTheme(this,
                Settings.Default.Theme == ThemeEnum.LightTheme ? "Light.Cyan" : "Dark.Cyan");
        }
    }
}