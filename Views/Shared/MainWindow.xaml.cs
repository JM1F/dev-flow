using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using dev_flow.Enums;
using dev_flow.Managers.Navigation;
using dev_flow.ViewModels.Shared;
using MahApps.Metro.Controls;

namespace dev_flow.Views.Shared
{
    public partial class MainWindow : MetroWindow
    {
        private readonly NavigationManager _navigationManager;

        public MainWindow()
        {
            InitializeComponent();

            IntPtr hWnd =
                new WindowInteropHelper(GetWindow(this) ?? throw new InvalidOperationException()).EnsureHandle();
            var attribute = Dwmwindowattribute.DwmwaWindowCornerPreference;
            var preference = DwmWindowCornerPreference.DwmwcpRound;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

            _navigationManager = new NavigationManager();
            _navigationManager.Navigated += NavigationServiceEx_OnNavigated;
            HamburgerMenuControl.Content = _navigationManager.Frame;

            // Navigate to the home page by default.
            Loaded += (sender, args) =>
                _navigationManager.Navigate(new Uri("Views/HomePageView.xaml", UriKind.RelativeOrAbsolute));
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
            Dwmwindowattribute attribute,
            ref DwmWindowCornerPreference pvAttribute,
            uint cbAttribute);

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is MenuItem { IsNavigation: true } menuItem)
            {
                _navigationManager.Navigate(menuItem.NavigationDestination);
            }
        }

        private void NavigationServiceEx_OnNavigated(object sender, NavigationEventArgs e)
        {
            // Select menu item
            HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedItemProperty,
                HamburgerMenuControl.Items
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));

            HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedOptionsItemProperty,
                HamburgerMenuControl
                    .OptionsItems
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));

            // Update back button
            GoBackButton.SetCurrentValue(VisibilityProperty,
                _navigationManager.CanGoBack ? Visibility.Visible : Visibility.Collapsed);
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            _navigationManager.GoBack();
        }
    }
}