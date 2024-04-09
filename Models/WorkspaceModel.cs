using System;

namespace dev_flow.Models;

/// <summary>
/// Represents a workspace in the application.
/// </summary>
public class WorkspaceModel
{
    /// <summary>
    /// Gets or sets the name of the workspace.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the workspace is marked as favourite.
    /// </summary>
    public bool IsFavourite { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the workspace is visible.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the full path of the workspace.
    /// </summary>
    public string FullWorkspacePath { get; set; }

    /// <summary>
    /// Gets or sets the ID of the workspace.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets the date when the workspace was last modified.
    /// </summary>
    public DateTime DateModified { get; set; }
}