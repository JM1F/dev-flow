using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace dev_flow.ViewModels.Shared;

public class MenuItem : HamburgerMenuIconItem
{
    public static readonly DependencyProperty NavigationDestinationProperty
        = DependencyProperty.Register(
            nameof(NavigationDestination),
            typeof(Uri),
            typeof(MenuItem),
            new PropertyMetadata(default(Uri)));

    public Uri NavigationDestination
    {
        get => (Uri)GetValue(NavigationDestinationProperty);
        set => SetValue(NavigationDestinationProperty, value);
    }

    public static readonly DependencyProperty NavigationTypeProperty
        = DependencyProperty.Register(
            nameof(NavigationType),
            typeof(Type),
            typeof(MenuItem),
            new PropertyMetadata(default(Type)));

    public Type NavigationType
    {
        get => (Type)GetValue(NavigationTypeProperty);
        set => SetValue(NavigationTypeProperty, value);
    }

    public bool IsNavigation => NavigationDestination != null;
}