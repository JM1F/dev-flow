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

    private void SortDoneTasksByCreationDate()
    {
        var sortedCreationDateDoneTasks = DoneTasks.OrderByDescending(task => task.DateCreated).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedCreationDateDoneTasks);
    }

    private void SortDoneTasksAlphabetically()
    {
        var sortedAlphabeticallyDoneTasks = DoneTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyDoneTasks.Reverse();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedAlphabeticallyDoneTasks);
    }

    private void SortDoneTasksByDueDate()
    {
        var sortedDueDateDoneTasks = DoneTasks.OrderByDescending(task => task.DueDate).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedDueDateDoneTasks);
    }

    private void SortDoneTasksBySeverity()
    {
        var sortedSeverityDoneTasks = DoneTasks.OrderByDescending(task => task.Severity).ToList();

        DoneTasks.Clear();
        DoneTasks.AddRange(sortedSeverityDoneTasks);
    }

    private void SortDoingTasksByCreationDate()
    {
        var sortedCreationDateDoingTasks = DoingTasks.OrderByDescending(task => task.DateCreated).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedCreationDateDoingTasks);
    }

    private void SortDoingTasksAlphabetically()
    {
        var sortedAlphabeticallyDoingTasks = DoingTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyDoingTasks.Reverse();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedAlphabeticallyDoingTasks);
    }

    private void SortDoingTasksByDueDate()
    {
        var sortedDueDateDoingTasks = DoingTasks.OrderByDescending(task => task.DueDate).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedDueDateDoingTasks);
    }

    private void SortDoingTasksBySeverity()
    {
        var sortedSeverityDoingTasks = DoingTasks.OrderByDescending(task => task.Severity).ToList();

        DoingTasks.Clear();
        DoingTasks.AddRange(sortedSeverityDoingTasks);
    }

    private void SortTodoTasksByCreationDate()
    {
        var sortedCreationDateTodoTasks = TodoTasks.OrderByDescending(task => task.DateCreated).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedCreationDateTodoTasks);
    }

    private void SortTodoTasksAlphabetically()
    {
        var sortedAlphabeticallyTodoTasks = TodoTasks.OrderByDescending(task => task.Title).ToList();
        sortedAlphabeticallyTodoTasks.Reverse();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedAlphabeticallyTodoTasks);
    }

    private void SortTodoTasksByDueDate()
    {
        var sortedDueDateTodoTasks = TodoTasks.OrderByDescending(task => task.DueDate).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedDueDateTodoTasks);
    }

    private void SortTodoTasksBySeverity()
    {
        var sortedSeverityTodoTasks = TodoTasks.OrderByDescending(task => task.Severity).ToList();

        TodoTasks.Clear();
        TodoTasks.AddRange(sortedSeverityTodoTasks);
    }


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

            tasksElement?.AddFirst(newTaskElement);

            await using var fileStream = new FileStream(xmlFilePath, FileMode.Create);
            await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
            fileStream.Close();
            await fileStream.DisposeAsync();
        }
    }


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

    private async Task SaveXmlAsync(XDocument xmlDoc, string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
        await xmlDoc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
        fileStream.Close();
        await fileStream.DisposeAsync();
    }

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


    private async Task LoadKanbanTasksAsync()
    {
        XDocument xmlDoc = await LoadXmlAsync(DevFlowConstants.KanbanBoardFileName);

        var kanbanTypes = xmlDoc.Root.Elements("KanbanTypes").Elements("KanbanType");

        foreach (var kanbanType in kanbanTypes)
        {
            string kanbanTypeName = kanbanType.Element("Name").Value;

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

    private async Task<XDocument> LoadXmlAsync(string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var xmlDoc = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
        fileStream.Close();
        await fileStream.DisposeAsync();
        return xmlDoc;
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: KanbanTask, TargetCollection: not null })
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
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
                        if (dropInfo.InsertIndex >= 0 && dropInfo.InsertIndex < sourceCollection.Count)
                        {
                            sourceCollection.Insert(dropInfo.InsertIndex, task);
                        }
                        else
                        {
                            // Handle invalid insert index, e.g., append to the end of the collection
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
                        // Handle invalid insert index, e.g., append to the end of the collection
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