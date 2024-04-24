using System.Collections.Generic;
using System.Xml.Serialization;

namespace dev_flow.Models;

/// <summary>
/// Represents a Kanban type.
/// </summary>
public class KanbanType
{
    [XmlElement("Name")] public string Name { get; set; }

    [XmlArray("KanbanTasks")]
    [XmlArrayItem("KanbanTask")]
    public List<KanbanTask> Tasks { get; set; }
}