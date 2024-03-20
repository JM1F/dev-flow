using System;
using System.Collections.ObjectModel;
using dev_flow.Views;
using MahApps.Metro.IconPacks;

namespace dev_flow.ViewModels.Shared;

public class SideMenuBarViewModel : ViewModelBase
{  
    public ObservableCollection<MenuItem> Menu { get; } = new();

        public ObservableCollection<MenuItem> OptionsMenu { get; } = new();

        public SideMenuBarViewModel()
        {
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
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CodeBranchSolid },
                Label = "Source Control",
                NavigationType = typeof(SourceControlPageView),
                NavigationDestination = new Uri("Views/SourceControlPageView.xaml", UriKind.RelativeOrAbsolute)
            });


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