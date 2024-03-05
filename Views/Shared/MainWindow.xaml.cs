using System.Windows;
using dev_flow.Managers;
using dev_flow.ViewModels;

namespace dev_flow.Views.Shared
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}