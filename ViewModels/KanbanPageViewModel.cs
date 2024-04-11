using System;
using System.Collections.ObjectModel;
using System.Windows;
using dev_flow.Models;
using dev_flow.ViewModels.Shared;
using GongSolutions.Wpf.DragDrop;

namespace dev_flow.ViewModels;

public class KanbanPageViewModel : ViewModelBase, IDropTarget
{
    public ObservableCollection<KanbanTask> TodoTasks { get; set; }
    public ObservableCollection<KanbanTask> DoingTasks { get; set; }
    public ObservableCollection<KanbanTask> DoneTasks { get; set; }

    public KanbanPageViewModel()
    {
        TodoTasks = new ObservableCollection<KanbanTask>();
        DoingTasks = new ObservableCollection<KanbanTask>();
        DoneTasks = new ObservableCollection<KanbanTask>();

        TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });
        
        TodoTasks.Add(new KanbanTask
        {
            Title = "Create a 323232332 project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });
        TodoTasks.Add(new KanbanTask
        {
            Title = "Create a 123123 project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a 3333333new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
                   {
                       Title = "Create a new project",
                       Description = "Create a new project in Visual Studio.",
                       DueDate = new DateTime(2022, 12, 31),
                       Criticality = 1,
                       Status = "To Do"
                   });TodoTasks.Add(new KanbanTask
                              {
                                  Title = "Create a new project",
                                  Description = "Create a new project in Visual Studio.",
                                  DueDate = new DateTime(2022, 12, 31),
                                  Criticality = 1,
                                  Status = "To Do"
                              });TodoTasks.Add(new KanbanTask
                                         {
                                             Title = "Create a new project",
                                             Description = "Create a new project in Visual Studio.",
                                             DueDate = new DateTime(2022, 12, 31),
                                             Criticality = 1,
                                             Status = "To Do"
                                         });TodoTasks.Add(new KanbanTask
                                                    {
                                                        Title = "Create a new project",
                                                        Description = "Create a new project in Visual Studio.",
                                                        DueDate = new DateTime(2022, 12, 31),
                                                        Criticality = 1,
                                                        Status = "To Do"
                                                    });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a 12331 project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
                   {
                       Title = "Create a new project",
                       Description = "Create a new project in Visual Studio.",
                       DueDate = new DateTime(2022, 12, 31),
                       Criticality = 1,
                       Status = "To Do"
                   });TodoTasks.Add(new KanbanTask
                              {
                                  Title = "Create a new project",
                                  Description = "Create a new project in Visual Studio.",
                                  DueDate = new DateTime(2022, 12, 31),
                                  Criticality = 1,
                                  Status = "To Do"
                              });TodoTasks.Add(new KanbanTask
                                         {
                                             Title = "Create a new project",
                                             Description = "Create a new project in Visual Studio.",
                                             DueDate = new DateTime(2022, 12, 31),
                                             Criticality = 1,
                                             Status = "To Do"
                                         });TodoTasks.Add(new KanbanTask
                                                    {
                                                        Title = "Create a new project",
                                                        Description = "Create a new project in Visual Studio.",
                                                        DueDate = new DateTime(2022, 12, 31),
                                                        Criticality = 1,
                                                        Status = "To Do"
                                                    });TodoTasks.Add(new KanbanTask
                                                               {
                                                                   Title = "Create a new project",
                                                                   Description = "Create a new project in Visual Studio.",
                                                                   DueDate = new DateTime(2022, 12, 31),
                                                                   Criticality = 1,
                                                                   Status = "To Do"
                                                               });TodoTasks.Add(new KanbanTask
                                                                          {
                                                                              Title = "Create a new project",
                                                                              Description = "Create a new project in Visual Studio.",
                                                                              DueDate = new DateTime(2022, 12, 31),
                                                                              Criticality = 1,
                                                                              Status = "To Do"
                                                                          });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });TodoTasks.Add(new KanbanTask
                   {
                       Title = "Create a new project",
                       Description = "Create a new project in Visual Studio.",
                       DueDate = new DateTime(2022, 12, 31),
                       Criticality = 1,
                       Status = "To Do"
                   });TodoTasks.Add(new KanbanTask
        {
            Title = "2313 a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "To Do"
        });

        DoneTasks.Add(new KanbanTask
        {
            Title = "Create a new 1123",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "Done"
        });

        DoneTasks.Add(new KanbanTask
        {
            Title = "Create a new project",
            Description = "Create a new project in Visual Studio.",
            DueDate = new DateTime(2022, 12, 31),
            Criticality = 1,
            Status = "Done"
        });
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is KanbanTask task)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is KanbanTask task)
        {
            var sourceCollection = dropInfo.DragInfo.SourceCollection as ObservableCollection<KanbanTask>;
            var targetCollection = dropInfo.TargetCollection as ObservableCollection<KanbanTask>;

            if (sourceCollection != null && targetCollection != null)
            {
                sourceCollection.Remove(task);
                targetCollection.Add(task);

                task.Status = GetStatusFromCollection(targetCollection);
                // Save XML
            }
        }
    }

    private string GetStatusFromCollection(ObservableCollection<KanbanTask> collection)
    {
        if (collection == TodoTasks)
            return "Todo";
        else if (collection == DoingTasks)
            return "Doing";
        else if (collection == DoneTasks)
            return "Done";
        else
            return string.Empty;
    }
}