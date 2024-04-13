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