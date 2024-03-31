using System.Windows;
using System.Windows.Controls;

namespace dev_flow.Assets.Styles;

public partial class WorkspaceCard : UserControl
{
    public static readonly RoutedEvent WorkspaceEnteredEvent =
        EventManager.RegisterRoutedEvent("WorkspaceEnteredEvent", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(WorkspaceCard));
    
    public event RoutedEventHandler WorkspaceEntered
    {
        add { AddHandler(WorkspaceEnteredEvent, value); }
        remove { RemoveHandler(WorkspaceEnteredEvent, value); }
    }
    
    public WorkspaceCard()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(WorkspaceEnteredEvent));
    }
}