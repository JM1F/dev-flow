using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using dev_flow.Assets.Styles;
using dev_flow.Enums;
using dev_flow.Helpers;
using dev_flow.Managers.Navigation;
using dev_flow.ViewModels;
using MahApps.Metro.Controls;
using MenuItem = dev_flow.ViewModels.Shared.MenuItem;

namespace dev_flow.Views.Shared
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly NavigationManager _navigationManager;

        public MainWindow()
        {
            InitializeComponent();

            // Subscribe to the WorkspaceItemSelected event
            WorkspaceItem.WorkspaceItemSelected += WorkspaceCard_OnWorkspaceEntered;

            // Check if the OS is Windows 11 or later
            if (IsWindows11OrLater())
            {
                IntPtr hWnd = new WindowInteropHelper(GetWindow(this) ?? throw new InvalidOperationException())
                    .EnsureHandle();
                var attribute = Dwmwindowattribute.DwmwaWindowCornerPreference;
                var preference = DwmWindowCornerPreference.DwmwcpRound;
                // Set the window attribute to round the corners
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }

            _navigationManager = new NavigationManager();
            // Subscribe to the Navigated event
            NavigationManager.Navigated += NavigationServiceEx_OnNavigated;
            HamburgerMenuControl.Content = _navigationManager.Frame;

            // Navigate to the home page by default
            Loaded += (sender, args) =>
                _navigationManager.Navigate(new Uri("Views/HomePageView.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Checks if the operating system is Windows 11 or later.
        /// </summary>
        /// <returns>True if the operating system is Windows 11 or later, false otherwise.</returns>
        private bool IsWindows11OrLater()
        {
            var osVersion = Environment.OSVersion.Version;
            return osVersion.Major >= 10 && osVersion.Build >= 22000; // Build 22000 is Windows 11
        }

        /// <summary>
        /// Handles the WorkspaceItemSelected event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void WorkspaceCard_OnWorkspaceEntered(object? sender, WorkspaceItemEventArgs e)
        {
            var workspaceItemObject = e.WorkspaceItem;
            _navigationManager.Navigate(new Uri("Views/WorkspaceEditorView.xaml", UriKind.RelativeOrAbsolute),
                workspaceItemObject);
        }

        // Define the DwmSetWindowAttribute function if the OS is Windows 11 or later
#if WINDOWS11_OR_LATER
        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd,
            Dwmwindowattribute attribute,
            ref DwmWindowCornerPreference pvAttribute,
            uint cbAttribute);
#endif

        /// <summary>
        /// Handles the ItemInvoked event of the HamburgerMenuControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is MenuItem { IsNavigation: true } menuItem)
            {
                // Navigate to the selected menu item
                _navigationManager.Navigate(menuItem.NavigationDestination);
            }
        }

        /// <summary>
        /// Handles the Navigated event of the NavigationManager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Handles the Click event of the GoBackButton.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            _navigationManager.GoBack();
        }
    }
}