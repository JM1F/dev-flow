using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Interfaces;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace dev_flow.ViewModels;

/// <summary>
/// View model for the workspace file explorer.
/// </summary>
public class WorkspaceFileExplorerViewModel : ViewModelBase
{
    private readonly DialogCoordinator _dialogCoordinator;
    private string _workspaceDocumentsPath;
    private WorkspaceItem _workspaceItem;

    // Event that is triggered when the delete button is clicked.
    public event EventHandler DeleteButtonClicked;


    // Properties

    private ObservableCollection<WorkspaceFile> _fileItems;

    public ObservableCollection<WorkspaceFile> FileItems
    {
        get => _fileItems;
        set
        {
            _fileItems = value;
            OnPropertyChanged(nameof(FileItems));
        }
    }

    private WorkspaceFile _selectedFileItem;

    public WorkspaceFile SelectedFileItem
    {
        get => _selectedFileItem;
        set
        {
            _selectedFileItem = value;
            OnPropertyChanged(nameof(SelectedFileItem));
        }
    }

    private string _documentSearchTerm;

    public string DocumentSearchTerm
    {
        get => _documentSearchTerm;
        set
        {
            _documentSearchTerm = value;
            OnPropertyChanged(nameof(DocumentSearchTerm));
            IsSearchNotPopulated = string.IsNullOrWhiteSpace(_documentSearchTerm);
        }
    }

    private bool _isNoDocumentsFound;

    public bool IsNoDocumentsFound
    {
        get => _isNoDocumentsFound;
        set
        {
            _isNoDocumentsFound = value;
            OnPropertyChanged(nameof(IsNoDocumentsFound));
        }
    }

    private bool _isSearchNotPopulated = true;

    public bool IsSearchNotPopulated
    {
        get => _isSearchNotPopulated;
        set
        {
            _isSearchNotPopulated = value;
            OnPropertyChanged(nameof(IsSearchNotPopulated));
        }
    }

    // Commands

    public ICommand OpenDirectoryCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand AddFileCommand { get; }
    public ICommand DeleteFileCommand { get; }
    public ICommand DocumentSearchCommand { get; }


    public WorkspaceFileExplorerViewModel(WorkspaceItem workspaceItem)
    {
        _workspaceItem = workspaceItem;
        _workspaceDocumentsPath = Path.GetFullPath(_workspaceItem.TrimmedBaseWorkspacePath + "/" +
                                                   _workspaceItem.TrimmedDocumentsWorkspacePath);

        _dialogCoordinator = new DialogCoordinator();

        FileItems = new ObservableCollection<WorkspaceFile>();
        OpenDirectoryCommand = new RelayCommand(OpenDocumentsDirectory);
        OpenFileCommand = new ParameterRelayCommand<WorkspaceFile>(OpenFile, CanOpenFile);
        AddFileCommand = new RelayCommand(AddFile);
        DeleteFileCommand = new ParameterRelayCommand<WorkspaceFile>(DeleteFile, CanDeleteFile);
        DocumentSearchCommand = new RelayCommand(SearchDocuments);
    }

    /// <summary>
    /// Searches for documents in the workspace based on the search term.
    /// </summary>
    private void SearchDocuments()
    {
        if (string.IsNullOrWhiteSpace(DocumentSearchTerm))
        {
            // If the search term is empty or null, display all files and folders asynchronously
            OpenDocumentsDirectory();
        }
        else
        {
            // Clear the existing file items
            FileItems.Clear();

            // Get the files and folders matching the search term asynchronously
            _ = GetFilesAndFoldersMatchingSearchTerm(_workspaceDocumentsPath, DocumentSearchTerm);
        }
    }

    /// <summary>
    /// Asynchronously gets the files and folders in the workspace that match the search term.
    /// </summary>
    /// <param name="workspaceDocumentsPath">The path of the workspace documents.</param>
    /// <param name="searchTerm">The search term.</param>
    private async Task GetFilesAndFoldersMatchingSearchTerm(string workspaceDocumentsPath, string searchTerm)
    {
        try
        {
            var filesTask = Task.Run(() => Directory.EnumerateFiles(workspaceDocumentsPath));
            var directoriesTask = Task.Run(() => Directory.EnumerateDirectories(workspaceDocumentsPath));

            // Wait for both tasks to complete
            await Task.WhenAll(filesTask, directoriesTask);

            var files = await filesTask;
            var directories = await directoriesTask;

            // Filter the files and folders based on the search term
            foreach (var file in files)
            {
                if (Path.GetFileName(file).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    // Add the file to the FileItems collection
                    FileItems.Add(new WorkspaceFile
                    {
                        Name = Path.GetFileName(file),
                        FullPath = file,
                        IsDirectory = false
                    });
                }
            }

            // Filter the directories based on the search term
            foreach (var dir in directories)
            {
                if (Path.GetFileName(dir).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    // Add the directory to the FileItems collection
                    FileItems.Add(new WorkspaceFile
                    {
                        Name = Path.GetFileName(dir),
                        FullPath = dir,
                        IsDirectory = true
                    });
                }
            }

            IsNoDocumentsFound = FileItems.Count == 0;
        }
        catch (UnauthorizedAccessException ex)
        {
            // Log and handle the exception
            Console.WriteLine($"Search - Unauthorised access: {ex.Message}");
            await _dialogCoordinator.ShowMessageAsync(this, "Access Denied",
                "You don't have permission to access this directory.");
        }
        catch (Exception ex)
        {
            // Log and handle any other exceptions
            Console.WriteLine($"Search - Error getting files and folders: {ex.Message}");
            await _dialogCoordinator.ShowMessageAsync(this, "Error",
                "An error occurred while getting files and folders.");
        }
    }

    /// <summary>
    /// Opens the documents directory and displays all files and folders.
    /// </summary>
    private void OpenDocumentsDirectory()
    {
        FileItems.Clear();
        // Get the files and folders in the workspace documents directory asynchronously
        _ = GetFilesAndFolders(_workspaceDocumentsPath);
    }

    /// <summary>
    /// Asynchronously gets all the files and folders in the workspace.
    /// </summary>
    /// <param name="directory">The directory to get the files and folders from.</param>
    private async Task GetFilesAndFolders(string directory)
    {
        try
        {
            var filesTask = Task.Run(() => Directory.EnumerateFiles(directory));
            var directoriesTask = Task.Run(() => Directory.EnumerateDirectories(directory));

            // Wait for both tasks to complete
            await Task.WhenAll(filesTask, directoriesTask);

            var files = await filesTask;
            var directories = await directoriesTask;

            // Add the files and directories to the FileItems collection
            foreach (var file in files)
            {
                FileItems.Add(new WorkspaceFile
                {
                    Name = Path.GetFileName(file),
                    FullPath = file,
                    IsDirectory = false
                });
            }

            // Add the directories to the FileItems collection
            foreach (var dir in directories)
            {
                FileItems.Add(new WorkspaceFile
                {
                    Name = Path.GetFileName(dir),
                    FullPath = dir,
                    IsDirectory = true
                });
            }

            IsNoDocumentsFound = FileItems.Count == 0;
        }
        catch (UnauthorizedAccessException ex)
        {
            // Log and handle the unauthorised access exception
            Console.WriteLine($"Unauthorised access: {ex.Message}");
            await _dialogCoordinator.ShowMessageAsync(this, "Error Getting File: Access Denied",
                "You don't have permission to access this file or directory.");
        }
        catch (Exception ex)
        {
            // Log and handle any other exceptions
            Console.WriteLine($"Error getting files and folders: {ex.Message}");
            await _dialogCoordinator.ShowMessageAsync(this, "Error Getting File: General Error",
                "An error occurred while getting the file or folder.");
        }
    }

    /// <summary>
    /// Checks if the selected file can be opened.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the selected file can be opened, false otherwise.</returns>
    private bool CanOpenFile(object obj)
    {
        return SelectedFileItem is { IsDirectory: false };
    }

    private void OpenFile(object obj)
    {
        if (SelectedFileItem is { IsDirectory: false })
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(SelectedFileItem.FullPath)
            {
                UseShellExecute = true
            });
        }
    }

    /// <summary>
    /// Adds a file to the workspace.
    /// </summary>
    private async void AddFile()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            var selectedFilePath = openFileDialog.FileName;
            var fileName = Path.GetFileName(selectedFilePath);
            var destinationFilePath = Path.Combine(_workspaceDocumentsPath, fileName);

            // Check if the file already exists in the FileItems collection
            if (FileItems.Any(f => f.FullPath == destinationFilePath))
            {
                // File already exists, display a message or handle it as needed
                Console.WriteLine($"File '{fileName}' already exists in the documents.");

                await _dialogCoordinator.ShowMessageAsync(this, "Document Importation Error",
                    "The file already exists in the documents. Change document name and try again.");
            }
            else
            {
                try
                {
                    // Copy the file to the workspace documents directory
                    await Task.Run(() => File.Copy(selectedFilePath, destinationFilePath, overwrite: true));

                    // Add the file to the FileItems collection
                    FileItems.Add(new WorkspaceFile
                    {
                        Name = fileName,
                        FullPath = destinationFilePath,
                        IsDirectory = false
                    });
                    // Update the workspace date modified
                    _workspaceItem.UpdateWorkspaceDateModified();
                    SearchDocuments();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Log and handle unauthorised access exception
                    Console.WriteLine($"Unauthorised access: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Copy: Access Denied",
                        "You don't have permission to copy this file or directory.");
                }
                catch (IOException ex)
                {
                    // Log and handle IO exception
                    Console.WriteLine($"Error copying file or directory: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Copy: IO Error",
                        "An error occurred while deleting the file or directory.");
                }
                catch (Exception ex)
                {
                    // Log and handle any other exceptions
                    Console.WriteLine($"Error copying file or directory: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Copy: General Error",
                        "An error occurred while copying the file or directory.");
                }
            }
        }
    }

    /// <summary>
    /// Checks if the selected file can be deleted.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the selected file can be deleted, false otherwise.</returns>
    private bool CanDeleteFile(object obj)
    {
        return SelectedFileItem != null;
    }

    /// <summary>
    /// Invokes the DeleteButtonClicked event.
    /// </summary>
    protected virtual void OnDeleteButtonClicked()
    {
        DeleteButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Deletes the selected file from the workspace.
    /// </summary>
    /// <param name="obj">The object to delete.</param>
    private async void DeleteFile(object obj)
    {
        // Invoke the DeleteButtonClicked event
        OnDeleteButtonClicked();

        // Check if the object is a WorkspaceFile
        if (obj is WorkspaceFile fileItem)
        {
            // Show a confirmation dialog before deleting the file
            var dialogResult =
                await _dialogCoordinator.ShowMessageAsync(this, "Delete Document",
                    "Deleting a document cannot be undone.\n\nAre you sure you want to delete this document?",
                    MessageDialogStyle.AffirmativeAndNegative);

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                try
                {
                    // Delete the file or directory
                    if (fileItem.IsDirectory)
                    {
                        await Task.Run(() => Directory.Delete(fileItem.FullPath, true));
                    }
                    else
                    {
                        await Task.Run(() => File.Delete(fileItem.FullPath));
                    }

                    FileItems.Remove(fileItem);

                    // Update the workspace date modified
                    _workspaceItem.UpdateWorkspaceDateModified();
                    SearchDocuments();
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Log and handle unauthorised access exception
                    Console.WriteLine($"Unauthorised access: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Deletion: Access Denied",
                        "You don't have permission to delete this file or directory.");
                }
                catch (IOException ex)
                {
                    // Log and handle IO exception
                    Console.WriteLine($"Error deleting file or directory: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Deletion: IO Error",
                        "An error occurred while deleting the file or directory.");
                }
                catch (Exception ex)
                {
                    // Log and handle any other exceptions
                    Console.WriteLine($"Error deleting file or directory: {ex.Message}");

                    await _dialogCoordinator.ShowMessageAsync(this, "File Deletion: General Error",
                        "An error occurred while deleting the file or directory.");
                }
            }
        }
    }
}