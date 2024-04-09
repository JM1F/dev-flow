using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using dev_flow.ViewModels;
using MahApps.Metro.Controls;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the HomePageView.xaml file.
/// </summary>
public partial class HomePageView
{
    public HomePageView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the selection changed event of the Tab Selector control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="Exception"></exception>
    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl tabControl)
        {
            // Check if the selected tab item is a MetroTabItem and its content is a UserControl
            if (tabControl.SelectedItem is MetroTabItem tabItem && tabItem.Content is UserControl userControl)
            {
                // Check the name of the user control and set the data context accordingly
                if (userControl.Name == "HomeWorkspaces")
                {
                    var homeWorkspacesViewModel = new BaseHomeWorkspacesViewModel();
                    userControl.DataContext = homeWorkspacesViewModel;
                    homeWorkspacesViewModel.HandleTabClick();
                }
                else if (userControl.Name == "HomeFavourites")
                {
                    var homeFavouritesViewModel = new HomeFavouritesViewModel();
                    userControl.DataContext = homeFavouritesViewModel;
                    homeFavouritesViewModel.HandleTabClick(true);
                }
            }
        }
        else
        {
            Console.WriteLine("Loading of Tab Content Failed!");
            // Throw an exception if the loading of the tab content failed
            throw new Exception("Loading of Tab Content Failed!");
        }
    }
}