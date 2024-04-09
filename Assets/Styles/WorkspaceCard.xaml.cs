using System.Windows;
using System.Windows.Controls;

namespace dev_flow.Assets.Styles;

/// <summary>
/// Code behind for the WorkspaceCard.xaml file
/// </summary>
public partial class WorkspaceCard : UserControl
{
    // Routed event for workspace entry
    public static readonly RoutedEvent WorkspaceEnteredEvent =
        EventManager.RegisterRoutedEvent("WorkspaceEnteredEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WorkspaceCard));

    // Event handler for workspace entry event
    public event RoutedEventHandler WorkspaceEntered
    {
        add { AddHandler(WorkspaceEnteredEvent, value); }
        remove { RemoveHandler(WorkspaceEnteredEvent, value); }
    }

    public WorkspaceCard()
    {
        InitializeComponent();
    }
}