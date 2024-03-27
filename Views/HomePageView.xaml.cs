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
        switch (e.Source)
        {
            case TabControl
            {
                SelectedItem: MetroTabItem
                {
                    Content: UserControl { DataContext: Home_WorkspacesViewModel homeWorkspacesViewModel }
                }
            }:
                homeWorkspacesViewModel.HandleTabClick();
                break;
            case TabControl
            {
                SelectedItem: MetroTabItem
                {
                    Content: UserControl { DataContext: Home_FavouritesViewModel homeFavouritesViewModel }
                }
            }:
                homeFavouritesViewModel.HandleTabClick();
                break;
            default:
                Console.WriteLine("Loading of Tab Content Failed!");
                throw new Exception("Loading of Tab Content Failed!");
        }
    }
}