﻿using System;

namespace dev_flow.Models;

public class WorkspaceModel
{
    public string Name { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsVisible { get; set; }
    public string FullWorkspacePath { get; set; }
    public int ID { get; set; }
    public DateTime DateModified { get; set; }
    
}