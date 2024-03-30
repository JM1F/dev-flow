using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using dev_flow.ViewModels;
using MahApps.Metro.Controls;

namespace dev_flow.Views;

public partial class HomePageView
{
    public HomePageView()
    {
        InitializeComponent();
        DataContext = new HomePageViewModel();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl tabControl)
        {
            if (tabControl.SelectedItem is MetroTabItem tabItem && tabItem.Content is UserControl userControl)
            {
                if (userControl.Name == "HomeWorkspaces")
                {
                    var homeWorkspacesViewModel = new Home_WorkspacesViewModel();
                    userControl.DataContext = homeWorkspacesViewModel;
                    homeWorkspacesViewModel.HandleTabClick();
                }
                else if (userControl.Name == "HomeFavourites")
                {
                    var homeFavouritesViewModel = new Home_FavouritesViewModel();
                    userControl.DataContext = homeFavouritesViewModel;
                    homeFavouritesViewModel.HandleTabClick();
                }
            }
        }
        else
        {
            Console.WriteLine("Loading of Tab Content Failed!");
            throw new Exception("Loading of Tab Content Failed!");
        }
    }
}