using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace dev_flow.ViewModels.Shared
{
    /// <summary>
    /// Represents a menu item in the application's navigation system.
    /// </summary>
    public class MenuItem : HamburgerMenuIconItem
    {
        /// <summary>
        /// Identifies the NavigationDestination dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationDestinationProperty
            = DependencyProperty.Register(
                nameof(NavigationDestination),
                typeof(Uri),
                typeof(MenuItem),
                new PropertyMetadata(default(Uri)));

        /// <summary>
        /// Gets or sets the destination URI that the menu item navigates to when clicked.
        /// </summary>
        public Uri NavigationDestination
        {
            get => (Uri)GetValue(NavigationDestinationProperty);
            set => SetValue(NavigationDestinationProperty, value);
        }

        /// <summary>
        /// Identifies the NavigationType dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationTypeProperty
            = DependencyProperty.Register(
                nameof(NavigationType),
                typeof(Type),
                typeof(MenuItem),
                new PropertyMetadata(default(Type)));

        /// <summary>
        /// Gets or sets the type of the destination that the menu item navigates to when clicked.
        /// </summary>
        public Type NavigationType
        {
            get => (Type)GetValue(NavigationTypeProperty);
            set => SetValue(NavigationTypeProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the menu item is a navigation item.
        /// </summary>
        public bool IsNavigation => NavigationDestination != null;
    }
}