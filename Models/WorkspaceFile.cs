namespace dev_flow.Models;

/// <summary>
/// Represents a file or directory within a workspace.
/// </summary>
public class WorkspaceFile
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public bool IsDirectory { get; set; }
}