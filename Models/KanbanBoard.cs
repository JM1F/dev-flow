using System.Collections.Generic;
using System.Xml.Serialization;

namespace dev_flow.Models;

[XmlRoot("KanbanBoard")]
public class KanbanBoard
{
    [XmlArray("KanbanTypes")]
    [XmlArrayItem("KanbanType")]
    public List<KanbanType> Types { get; set; }
}