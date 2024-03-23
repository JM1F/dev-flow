using System.Windows.Controls;

namespace dev_flow.Views;

public partial class GeneralDisplayBoxControl : UserControl
{
    public GeneralDisplayBoxControl()
    {
        InitializeComponent();
    }
    
    public string Title
    {
        get => TitleTextBlock.Text;
        set => TitleTextBlock.Text = value;
    }
    
}