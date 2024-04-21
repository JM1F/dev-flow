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

public class KanbanPageViewModel : ViewModelBase, IDropTarget
{
    private readonly DialogCoordinator _dialogCoordinator;

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

    public KanbanPageViewModel()
    {
        _dialogCoordinator = new DialogCoordinator();

        TodoTasks = new RangedObservableCollection<KanbanTask>();
        DoingTasks = new RangedObservableCollection<KanbanTask>();
        DoneTasks = new RangedObservableCollection<KanbanTask>();

        DeleteTaskCommand = new AsyncParameterRelayCommand<KanbanTask>(DeleteKanbanTask);
        AddTaskCommand = new AsyncParameterRelayCommand<KanbanStatusEnum>(AddKanbanTask);
        SaveAllTasksCommand = new AsyncRelayCommand(SaveTaskCollectionsToXmlAsync);

        _ = LoadKanbanTasksAsync();
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
                    new XElement("Status", task.Status)
                );
                doneTasksElement.Add(taskElement);
            }
        }

        // Save the modified XML document back to the file
        await using var fileStream = new FileStream(xmlFilePath, FileMode.Create);
        await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
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
                new XElement("Status", task.Status)
            );

            tasksElement?.Add(newTaskElement);

            await using var fileStream = new FileStream(xmlFilePath, FileMode.Create);
            await doc.SaveAsync(fileStream, SaveOptions.None, CancellationToken.None);
            fileStream.Close();
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
                    TodoTasks.Add(newTask);
                }

                break;
            }
            case KanbanStatusEnum.Doing:
            {
                var result = await DialogHost.Show(dialog, "DoingDialog", null, null);
                if (result is KanbanTask newTask)
                {
                    await SaveTaskToXmlAsync(newTask);
                    DoingTasks.Add(newTask);
                }

                break;
            }
            case KanbanStatusEnum.Done:
            {
                var result = await DialogHost.Show(dialog, "DoneDialog", null, null);
                if (result is KanbanTask newTask)
                {
                    await SaveTaskToXmlAsync(newTask);
                    DoneTasks.Add(newTask);
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
                    Status = (KanbanStatusEnum)Enum.Parse(typeof(KanbanStatusEnum), task.Element("Status").Value)
                });

            switch (kanbanTypeName)
            {
                case "ToDo":
                    TodoTasks = new RangedObservableCollection<KanbanTask>();
                    TodoTasks.AddRange(tasks);
                    break;
                case "Doing":
                    DoingTasks = new RangedObservableCollection<KanbanTask>();
                    DoingTasks.AddRange(tasks);
                    break;
                case "Done":
                    DoneTasks = new RangedObservableCollection<KanbanTask>();
                    DoneTasks.AddRange(tasks);
                    break;
            }
        }
    }

    private async Task<XDocument> LoadXmlAsync(string filePath)
    {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        return await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo is { Data: KanbanTask, TargetCollection: not null })
        {
            dropInfo.Effects = DragDropEffects.All;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is KanbanTask task && dropInfo.TargetCollection != null)
        {
            var sourceCollection = dropInfo.DragInfo.SourceCollection.TryGetList();
            var targetCollection = dropInfo.TargetCollection.TryGetList();

            if (sourceCollection != null && targetCollection != null && sourceCollection != targetCollection)
            {
                sourceCollection.Remove(task);
                targetCollection.Add(task);

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