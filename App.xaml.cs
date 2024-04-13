using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using ControlzEx.Theming;
using dev_flow.Enums;
using dev_flow.Models;
using dev_flow.Properties;
using Constants = dev_flow.Interfaces.Constants;

namespace dev_flow
{
    /// <summary>
    /// Code-behind for the App.xaml file.
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;

        /// <summary>
        /// Called when the application is started. This method is the entry point of the application.
        /// </summary>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Create a mutex to ensure only one instance of the application is running
            _mutex = new Mutex(true, Constants.MutexName, out var isNewAppInstance);

            // If another instance of the application is already running, show a message and close.
            if (!isNewAppInstance)
            {
                MessageBox.Show("Another instance of the application is already running.", "dev-flow",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
            CreateWorkspaceDirectory();
            CreateKanbanBoard();
            SetTheme();
        }

        /// <summary>
        /// Creates the kanban board .xml if it does not exist with an empty structure.
        /// </summary>
        private void CreateKanbanBoard()
        {
            var currentDirectory = Settings.Default.WorkspacePath;

            var kanbanPath = Path.Combine(currentDirectory, Constants.KanbanBoardFileName);

            if (!File.Exists(kanbanPath))
            {
                var emptyKanbanBoardTemplate = new KanbanBoard
                {
                    Types = new List<KanbanType>
                    {
                        new() { Name = "To-Do", Tasks = new List<KanbanTask>() },
                        new() { Name = "Doing", Tasks = new List<KanbanTask>() },
                        new() { Name = "Done", Tasks = new List<KanbanTask>() }
                    }
                };

                try
                {
                    using (StreamWriter writer = new StreamWriter(kanbanPath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(KanbanBoard));
                        serializer.Serialize(writer, emptyKanbanBoardTemplate);
                    }

                    Console.WriteLine("Kanban board created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating kanban board: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Kanban board already exists.");
            }
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


        /// <summary>
        /// Called when the application is exited.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Release the mutex upon application exit
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }

            base.OnExit(e);
        }
    }
}