﻿using System.Windows.Controls;
using dev_flow.ViewModels;

namespace dev_flow.Views;

public partial class SettingsPageView
{
    public SettingsPageView()
    {
        InitializeComponent();
        DataContext = new SettingsPageViewModel();
    }
}