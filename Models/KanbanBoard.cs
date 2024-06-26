﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace dev_flow.Models;

/// <summary>
/// Represents a kanban board.
/// </summary>
[XmlRoot("KanbanBoard")]
public class KanbanBoard
{
    [XmlArray("KanbanTypes")]
    [XmlArrayItem("KanbanType")]
    public List<KanbanType> Types { get; set; }
}