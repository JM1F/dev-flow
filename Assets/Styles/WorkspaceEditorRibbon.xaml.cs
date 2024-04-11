using System.Windows;
using System.Windows.Controls;

namespace dev_flow.Assets.Styles;

/// <summary>
/// Code behind for the WorkspaceEditorRibbon.xaml file
/// </summary>
public partial class WorkspaceEditorRibbon : UserControl
{
    public WorkspaceEditorRibbon()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        EditViewRadioButton.IsChecked = true;
    }
}