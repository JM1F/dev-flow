using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using dev_flow.Assets.Styles;
using dev_flow.Commands;
using dev_flow.Commands.dev_flow.Commands;
using dev_flow.Constants;
using dev_flow.Enums;
using dev_flow.Helpers;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;

namespace dev_flow.ViewModels;

public class KanbanPageViewModel : ViewModelBase, IDropTarget, IDisposable
{
    // Properties
    private readonly DialogCoordinator _dialogCoordinator;
    private bool _disposed;

    private RangedObservableCollection<KanbanTask> _todoTasks;
    private RangedObservableCollection<KanbanTask> _doingTasks;
    private RangedObservableCollection<KanbanTask> _doneTasks;

    public RangedObservableCollection<KanbanTask> TodoTasks
    {
        get { return _todoTasks; }
        set
        {
            _todoTasks = value;
            OnPropertyChanged(nameof(TodoTasks));
        }
    }

    public RangedObservableCollection<KanbanTask> DoingTasks
    {
        get { return _doingTasks; }
        set
        {
            _doingTasks = value;
            OnPropertyChanged(nameof(DoingTasks));
        }
    }

    public RangedObservableCollection<KanbanTask> DoneTasks
    {
        get { return _doneTasks; }
        set
        {
            _doneTasks = value;
            OnPropertyChanged(nameof(DoneTasks));
        }
    }

    // General Commands
    public ICommand DeleteTaskCommand { get; set; }
    public ICommand AddTaskCommand { get; set; }
    public ICommand SaveAllTasksCommand { get; set; }

    // To-do Sort Commands
    public ICommand SortTodoTasksBySeverityCommand { get; set; }
    public ICommand SortTodoTasksByDueDateCommand { get; set; }
    public ICommand SortTodoTasksAlphabeticallyCommand { get; set; }
    public ICommand SortTodoTasksByCreationDateCommand { get; set; }

    // Doing Sort Commands
    public ICommand SortDoingTasksBySeverityCommand { get; set; }
    public ICommand SortDoingTasksByDueDateCommand { get; set; }
    public ICommand SortDoingTasksAlphabeticallyCommand { get; set; }
    public ICommand SortDoingTasksByCreationDateCommand { get; set; }

    // Done Sort Commands
    public ICommand SortDoneTasksBySeverityCommand { get; set; }
    public ICommand SortDoneTasksByDueDateCommand { get; set; }
    public ICommand SortDoneTasksAlphabeticallyCommand { get; set; }
    public ICommand SortDoneTasksByCreationDateCommand { get; set; }


    public KanbanPageViewModel()
    {
        _dialogCoordinator = new DialogCoordinator();

        TodoTasks = new RangedObservableCollection<KanbanTask>();
        DoingTasks = new RangedObservableCollection<KanbanTask>();
        DoneTasks = new RangedObservableCollection<KanbanTask>();

        // General Commands
        DeleteTaskCommand = new AsyncParameterRelayCommand<KanbanTask>(DeleteKanbanTask);
        AddTaskCommand = new AsyncParameterRelayCommand<KanbanStatusEnum>(AddKanbanTask);
        SaveAllTasksCommand = new AsyncRelayCommand(SaveTaskCollectionsToXmlAsync);

        // To-do Sort Commands
        SortTodoTasksBySeverityCommand = new RelayCommand(SortTodoTasksBySeverity);
        SortTodoTasksByDueDateCommand = new RelayCommand(SortTodoTasksByDueDate);
        SortTodoTasksAlphabeticallyCommand = new RelayCommand(SortTodoTasksAlphabetically);
        SortTodoTasksByCreationDateCommand = new RelayCommand(SortTodoTasksByCreationDate);

        // Doing Sort Commands
        SortDoingTasksBySeverityCommand = new RelayCommand(SortDoingTasksBySeverity);
        SortDoingTasksByDueDateCommand = new RelayCommand(SortDoingTasksByDueDate);
        SortDoingTasksAlphabeticallyCommand = new RelayCommand(SortDoingTasksAlphabetically);
        SortDoingTasksByCreationDateCommand = new RelayCommand(SortDoingTasksByCreationDate);

        // Done Sort Commands
        SortDoneTasksBySeverityCommand = new RelayCommand(SortDoneTasksBySeverity);
        SortDoneTasksByDueDateCommand = new RelayCommand(SortDoneTasksByDueDate);
        SortDoneTasksAlphabeticallyCommand = new RelayCommand(SortDoneTasksAlphabetically);
        SortDoneTasksByCreationDateCommand = new RelayCommand(SortDoneTasksByCreationDate);

        _ = LoadKanbanTasksAsync();
    }

    /// <summary>
    /// Sorts the Done tasks by creation date.
    /// </summary>
    private void SortDoneTasksByCreationDate()
    {
        var sortedCreationDateDoneTasks = DoneTasks.OrderByDescending(task => task.DateCreated).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedCreationDateDoneTasks);
    }

    /// <summary>
    /// Sorts the Done tasks alphabetically.
    /// </summary>
    private void SortDoneTasksAlphabetically()
    {
        var sortedAlphabeticallyDoneTasks = DoneTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyDoneTasks.Reverse();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedAlphabeticallyDoneTasks);
    }

    /// <summary>
    /// Sorts the Done tasks by due date.
    /// </summary>
    private void SortDoneTasksByDueDate()
    {
        var sortedDueDateDoneTasks = DoneTasks.OrderByDescending(task => task.DueDate).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedDueDateDoneTasks);
    }

    /// <summary>
    /// Sorts the Done tasks by severity.
    /// </summary>
    private void SortDoneTasksBySeverity()
    {
        var sortedSeverityDoneTasks = DoneTasks.OrderByDescending(task => task.Severity).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedSeverityDoneTasks);
    }

    /// <summary>
    /// Sorts the Doing tasks by creation date.
    /// </summary>
    private void SortDoingTasksByCreationDate()
    {
        var sortedCreationDateDoingTasks = DoingTasks.OrderByDescending(task => task.DateCreated).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedCreationDateDoingTasks);
    }

    /// <summary>
    /// Sorts the Doing tasks alphabetically.
    /// </summary>
    private void SortDoingTasksAlphabetically()
    {
        var sortedAlphabeticallyDoingTasks = DoingTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyDoingTasks.Reverse();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedAlphabeticallyDoingTasks);
    }

    /// <summary>
    /// Sorts the Doing tasks by due date.
    /// </summary>
    private void SortDoingTasksByDueDate()
    {
        var sortedDueDateDoingTasks = DoingTasks.OrderByDescending(task => task.DueDate).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedDueDateDoingTasks);
    }

    /// <summary>
    /// Sorts the Doing tasks by severity.
    /// </summary>
    private void SortDoingTasksBySeverity()
    {
        var sortedSeverityDoingTasks = DoingTasks.OrderByDescending(task => task.Severity).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedSeverityDoingTasks);
    }

    /// <summary>
    /// Sorts the To-do tasks by creation date.
    /// </summary>
    private void SortTodoTasksByCreationDate()
    {
        var sortedCreationDateTodoTasks = TodoTasks.OrderByDescending(task => task.DateCreated).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedCreationDateTodoTasks);
    }

    /// <summary>
    /// Sorts the To-do tasks alphabetically.
    /// </summary>
    private void SortTodoTasksAlphabetically()
    {
        var sortedAlphabeticallyTodoTasks = TodoTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyTodoTasks.Reverse();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedAlphabeticallyTodoTasks);
    }

    /// <summary>
    /// Sorts the To-do tasks by due date.
    /// </summary>
    private void SortTodoTasksByDueDate()
    {
        var sortedDueDateTodoTasks = TodoTasks.OrderByDescending(task => task.DueDate).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedDueDateTodoTasks);
    }

    /// <summary>
    /// Sorts the To-do tasks by severity.
    /// </summary>
    private void SortTodoTasksBySeverity()
    {
        var sortedSeverityTodoTasks = TodoTasks.OrderByDescending(task => task.Severity).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedSeverityTodoTasks);
    }


    /// <summary>
    /// Saves the task collections to the XML file.
    /// </summary>
    private async Task SaveTaskCollectionsToXmlAsync()
    {
        var xmlFilePath = DevFlowConstants.KanbanBoardFileName;
        var doc = XDocument.Load(xmlFilePath);

        // Save TodoTasks
        var todoTasksElement = doc.Root.Element("KanbanTypes")
            .Elements("KanbanType")
            .FirstOrDefault(x => x.Element("Name").Value == "ToDo")?
            .Element("KanbanTasks");

        if (todoTasksElement != null)
        {
            todoTasksElement.RemoveAll();
            foreach (var task in TodoTasks)
            {
                var taskElement = new XElement("KanbanTask",
                    new XElement("ID", task.ID),
                    new XElement("Title", task.Title),
                    new XElement("Description", task.Description),
                    new XElement("Severity", task.Severity),
                    new XElement("DueDate", task.DueDate.ToString("yyyy-MM-dd")),
                    new XElement("DateCreated", task.DateCreated.ToString("yyyy-MM-dd")),
                    new XElement("Status", task.Status)
                );
                todoTasksElement.Add(taskElement);
            }
        }

        // Save DoingTasks
        var doingTasksElement = doc.Root.Element("KanbanTypes")
            .Elements("KanbanType")
            .FirstOrDefault(x => x.Element("Name").Value == "Doing")?
            .Element("KanbanTasks");

        if (doingTasksElement != null)
        {
            doingTasksElement.RemoveAll();
            foreach (var task in DoingTasks)
            {
                var taskElement = new XElement("KanbanTask",
                    new XElement("ID", task.ID),
                    new XElement("Title", task.Title),
                    new XElement("Description", task.Description),
                    new XElement("Severity", task.Severity),
                    new XElement("DueDate", task.DueDate.ToString("yyyy-MM-dd")),
                    new XElement("DateCreated", task.DateCreated.ToString("yyyy-MM-dd")),
                    new XElement("Status", task.Status)
                );
                doingTasksElement.Add(taskElement);
            }
        }

        // Save DoneTasks
        var doneTasksElement = doc.Root.Element("KanbanTypes")
            .Elements("KanbanType")
            .FirstOrDefault(x => x.Element("Name").Value == "Done")?
            .Element("KanbanTasks");

        if (doneTasksElement != null)
        {
            doneTasksElement.RemoveAll();
            foreach (var task in DoneTasks)
            {
                var taskElement = new XElement("KanbanTask",
                    new XElement("ID", task.ID),
                    new XElement("Title", task.Title),
                    new XElement("Description", task.Description),
                    new XElement("Severity", task.Severity),
                    new XElement("DueDate", task.DueDate.ToString("yyyy-MM-dd")),
                    new XElement("DateCreated", task.DateCreated.ToString("yyyy-MM-dd")),
                    new XElement("Status", task.Status)
                );
                doneTasksElement.Add(taskElement);
            }
        }

        // Save the modified XML document back to the file
        await using var fileStream = new FileStream(xmlFilePath, FileMode.Create);
        await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
        fileStream.Close();
        await fileStream.DisposeAsync();
    }

    /// <summary>
    /// Saves the task to the XML file.
    /// </summary>
    /// <param name="task"></param>
    private async Task SaveTaskToXmlAsync(KanbanTask task)
    {
        var xmlFilePath = DevFlowConstants.KanbanBoardFileName;
        var doc = XDocument.Load(xmlFilePath);

        var kanbanTypeElement = doc.Root
            .Element("KanbanTypes")
            .Elements("KanbanType")
            .FirstOrDefault(x => x.Element("Name").Value == task.Status.ToString());

        if (kanbanTypeElement != null)
        {
            var tasksElement = kanbanTypeElement.Element("KanbanTasks");

            var newTaskElement = new XElement("KanbanTask",
                new XElement("ID", task.ID),
                new XElement("Title", task.Title),
                new XElement("Description", task.Description),
                new XElement("Severity", task.Severity),
                new XElement("DueDate", task.DueDate.ToString("yyyy-MM-dd")),
                new XElement("DateCreated", task.DateCreated.ToString("yyyy-MM-dd")),
                new XElement("Status", task.Status)
            );
            // Add the new task to the beginning of the tasks list
            tasksElement?.AddFirst(newTaskElement);

            await using var fileStream = new FileStream(xmlFilePath, FileMode.Create);
            await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
            fileStream.Close();
            await fileStream.DisposeAsync();
        }
    }


    /// <summary>
    /// Adds a new task to the specified kanban board.
    /// </summary>
    /// <param name="kanbanBoard"></param>
    private async Task AddKanbanTask(KanbanStatusEnum kanbanBoard)
    {
        var dialog = new AddTaskDialog()
        {
            DataContext = new AddTasksDialogViewModel(kanbanBoard)
        };

        switch (kanbanBoard)
        {
            case KanbanStatusEnum.ToDo:
            {
                var result = await DialogHost.Show(dialog, "TodoDialog", null, null);
                if (result is KanbanTask newTask)
                {
                    await SaveTaskToXmlAsync(newTask);
                    TodoTasks.Insert(0, newTask);
                }

                break;
            }
            case KanbanStatusEnum.Doing:
            {
                var result = await DialogHost.Show(dialog, "DoingDialog", null, null);
                if (result is KanbanTask newTask)
                {
                    await SaveTaskToXmlAsync(newTask);
                    DoingTasks.Insert(0, newTask);
                }

                break;
            }
            case KanbanStatusEnum.Done:
            {
                var result = await DialogHost.Show(dialog, "DoneDialog", null, null);
                if (result is KanbanTask newTask)
                {
                    await SaveTaskToXmlAsync(newTask);
                    DoneTasks.Insert(0, newTask);
                }

                break;
            }
        }
    }

    /// <summary>
    /// Saves the XML document to the specified file path.
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="filePath"></param>
    private async Task SaveXmlAsync(XDocument xmlDoc, string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
        await xmlDoc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
        fileStream.Close();
        await fileStream.DisposeAsync();
    }

    /// <summary>
    /// Updates the XML file.
    /// </summary>
    /// <param name="taskToRemove"></param>
    private async Task UpdateXmlFileAsync(KanbanTask taskToRemove)
    {
        XDocument xmlDoc = await LoadXmlAsync(DevFlowConstants.KanbanBoardFileName);

        var kanbanTypes = xmlDoc.Root.Elements("KanbanTypes").Elements("KanbanType");

        foreach (var kanbanType in kanbanTypes)
        {
            var taskElement = kanbanType.Element("KanbanTasks").Elements("KanbanTask")
                .FirstOrDefault(task => Guid.Parse(task.Element("ID").Value) == taskToRemove.ID);

            if (taskElement != null)
            {
                taskElement.Remove();
                break;
            }
        }

        await SaveXmlAsync(xmlDoc, DevFlowConstants.KanbanBoardFileName);
    }

    /// <summary>
    /// Deletes the specified Kanban task.
    /// </summary>
    /// <param name="task"></param>
    private async Task DeleteKanbanTask(KanbanTask task)
    {
        if (task != null)
        {
            if (TodoTasks.Contains(task))
                TodoTasks.Remove(task);
            else if (DoingTasks.Contains(task))
                DoingTasks.Remove(task);
            else if (DoneTasks.Contains(task))
                DoneTasks.Remove(task);

            await UpdateXmlFileAsync(task);
        }
    }


    /// <summary>
    /// Loads the Kanban tasks from the XML file.
    /// </summary>
    private async Task LoadKanbanTasksAsync()
    {
        XDocument xmlDoc = await LoadXmlAsync(DevFlowConstants.KanbanBoardFileName);

        var kanbanTypes = xmlDoc.Root.Elements("KanbanTypes").Elements("KanbanType");

        foreach (var kanbanType in kanbanTypes)
        {
            string kanbanTypeName = kanbanType.Element("Name").Value;
            // Create new KanbanTask objects from the XML data
            var tasks = kanbanType.Element("KanbanTasks").Elements("KanbanTask")
                .Select(task => new KanbanTask
                {
                    ID = Guid.Parse(task.Element("ID").Value),
                    Title = task.Element("Title").Value,
                    Description = task.Element("Description").Value,
                    Severity = (KanbanSeverityEnum)Enum.Parse(typeof(KanbanSeverityEnum),
                        task.Element("Severity").Value),
                    DueDate = DateTime.Parse(task.Element("DueDate").Value),
                    DateCreated = DateTime.Parse(task.Element("DateCreated").Value),
                    Status = (KanbanStatusEnum)Enum.Parse(typeof(KanbanStatusEnum), task.Element("Status").Value)
                });

            // Add the tasks to the appropriate collection
            switch (kanbanTypeName)
            {
                case "ToDo":
                    TodoTasks.Clear();
                    TodoTasks.AddRange(tasks);
                    break;
                case "Doing":
                    DoingTasks.Clear();
                    DoingTasks.AddRange(tasks);
                    break;
                case "Done":
                    DoneTasks.Clear();
                    DoneTasks.AddRange(tasks);
                    break;
            }
        }
    }

    /// <summary>
    /// Loads the XML document asynchronously.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private async Task<XDocument> LoadXmlAsync(string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var xmlDoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
        fileStream.Close();
        await fileStream.DisposeAsync();
        return xmlDoc;
    }

    /// <summary>
    /// Handles the drag over event.
    /// </summary>
    /// <param name="dropInfo"></param>
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: KanbanTask, TargetCollection: not null })
        {
            // Allow the drop
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    /// <summary>
    /// Handles the drop event.
    /// </summary>
    /// <param name="dropInfo"></param>
    public void Drop(IDropInfo dropInfo)
    {
        // Check if the data is a KanbanTask and the target collection is not null
        if (dropInfo.Data is KanbanTask task && dropInfo.TargetCollection != null)
        {
            var sourceCollection = dropInfo.DragInfo.SourceCollection.TryGetList();
            var targetCollection = dropInfo.TargetCollection.TryGetList();

            if (sourceCollection != null && targetCollection != null)
            {
                if (sourceCollection == targetCollection)
                {
                    // Reorder within the same collection
                    var index = sourceCollection.IndexOf(task);
                    if (index != dropInfo.InsertIndex)
                    {
                        sourceCollection.RemoveAt(index);
                        // Adjust the insert index based on the removed item
                        if (dropInfo.InsertIndex >= 0 && dropInfo.InsertIndex < sourceCollection.Count)
                        {
                            sourceCollection.Insert(dropInfo.InsertIndex, task);
                        }
                        else
                        {
                            // Handle invalid insert index, append to the end of the collection
                            sourceCollection.Add(task);
                        }
                    }
                }
                else
                {
                    // Move between different collections
                    sourceCollection.Remove(task);
                    if (dropInfo.InsertIndex >= 0 && dropInfo.InsertIndex <= targetCollection.Count)
                    {
                        targetCollection.Insert(dropInfo.InsertIndex, task);
                    }
                    else
                    {
                        // Handle invalid insert index, append to the end of the collection
                        targetCollection.Add(task);
                    }

                    // Update the KanbanStatus based on the target collection
                    if (Equals(targetCollection, TodoTasks))
                    {
                        task.Status = KanbanStatusEnum.ToDo;
                    }
                    else if (Equals(targetCollection, DoingTasks))
                    {
                        task.Status = KanbanStatusEnum.Doing;
                    }
                    else if (Equals(targetCollection, DoneTasks))
                    {
                        task.Status = KanbanStatusEnum.Done;
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            TodoTasks.Clear();
            DoingTasks.Clear();
            DoneTasks.Clear();

            TodoTasks = null;
            DoneTasks = null;
            DoneTasks = null;
        }

        _disposed = true;
    }
}