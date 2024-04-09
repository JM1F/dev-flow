using System.Windows.Controls;
using dev_flow.ViewModels;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the AboutPageView.xaml file.
/// </summary>
public partial class AboutPageView : UserControl
{
    public AboutPageView()
    {
        InitializeComponent();
        DataContext = new AboutPageViewModel();
    }
}