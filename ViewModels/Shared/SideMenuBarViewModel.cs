using System;
using System.Collections.ObjectModel;
using dev_flow.Views;
using MahApps.Metro.IconPacks;

namespace dev_flow.ViewModels.Shared;

/// <summary>
/// View model for the side menu bar.
/// </summary>
public class SideMenuBarViewModel : ViewModelBase
{
    // Collection of the main menu items.
    public ObservableCollection<MenuItem> Menu { get; } = new();

    // Collection of the option menu items.
    public ObservableCollection<MenuItem> OptionsMenu { get; } = new();

    public SideMenuBarViewModel()
    {
        // Add menu items to the Menu collection
        Menu.Add(new MenuItem()
        {
            Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HomeSolid },
            Label = "Home",
            NavigationType = typeof(HomePageView),
            NavigationDestination = new Uri("Views/HomePageView.xaml", UriKind.RelativeOrAbsolute)
        });
        Menu.Add(new MenuItem()
        {
            Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.TasksSolid },
            Label = "Tasks",
            NavigationType = typeof(KanbanPageView),
            NavigationDestination = new Uri("Views/KanbanPageView.xaml", UriKind.RelativeOrAbsolute)
        });
        Menu.Add(new MenuItem()
        {
            Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CodeSolid },
            Label = "Code Sandbox",
            NavigationType = typeof(CodeSandboxPageView),
            NavigationDestination = new Uri("Views/CodeSandboxPageView.xaml", UriKind.RelativeOrAbsolute)
        });


        // Add menu items to the OptionsMenu collection
        OptionsMenu.Add(new MenuItem()
        {
            Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogsSolid },
            Label = "Settings",
            NavigationType = typeof(SettingsPageView),
            NavigationDestination = new Uri("Views/SettingsPageView.xaml", UriKind.RelativeOrAbsolute)
        });
        OptionsMenu.Add(new MenuItem()
        {
            Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoCircleSolid },
            Label = "About",
            NavigationType = typeof(AboutPageView),
            NavigationDestination = new Uri("Views/AboutPageView.xaml", UriKind.RelativeOrAbsolute)
        });
    }
}