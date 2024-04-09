using System;

namespace dev_flow.Models;

/// <summary>
/// Represents a task in a Kanban board.
/// </summary>
public class KanbanTask
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}