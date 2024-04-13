using System;
using System.Xml.Serialization;
using dev_flow.Enums;

namespace dev_flow.Models;

/// <summary>
/// Represents a task in a Kanban board.
/// </summary>
public class KanbanTask
{
    [XmlElement("Title")]
    public string Title { get; set; }

    [XmlElement("Description")]
    public string Description { get; set; }

    [XmlElement("Severity")]
    public string Severity { get; set; }

    [XmlElement("DueDate")]
    public DateTime DueDate { get; set; }
    
    [XmlElement("Status")]
    public KanbanStatusEnum Status { get; set; }
}