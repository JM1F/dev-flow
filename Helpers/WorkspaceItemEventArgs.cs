using dev_flow.ViewModels;

namespace dev_flow.Helpers;

public class WorkspaceItemEventArgs
{
    public WorkspaceItem WorkspaceItem { get; }

    public WorkspaceItemEventArgs(WorkspaceItem workspaceItem)
    {
        WorkspaceItem = workspaceItem;
    }
}