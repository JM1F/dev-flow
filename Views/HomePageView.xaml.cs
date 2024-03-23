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
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl
            {
                SelectedItem: MetroTabItem
                {
                    Content: UserControl { DataContext: Home_WorkspacesViewModel homeWorkspacesViewModel }
                }
            })
        {
            homeWorkspacesViewModel.HandleTabClick();
        }
        else if (e.Source is TabControl
                 {
                     SelectedItem: MetroTabItem
                     {
                         Content: UserControl { DataContext: Home_FavouritesViewModel homeFavouritesViewModel }
                     }
                 })
        {
            homeFavouritesViewModel.HandleTabClick();
        }
        else
        {
            Console.WriteLine("Loading of Tab Content Failed!");
            throw new Exception("Loading of Tab Content Failed!");
        }
    }
}