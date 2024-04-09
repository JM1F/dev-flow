using dev_flow.ViewModels;

namespace dev_flow.Helpers;

/// <summary>
/// Provides data for the WorkspaceItem event.
/// </summary>
public class WorkspaceItemEventArgs
{
    public WorkspaceItem WorkspaceItem { get; }

    public WorkspaceItemEventArgs(WorkspaceItem workspaceItem)
    {
        WorkspaceItem = workspaceItem;
    }
}