using System;
using System.Windows;
using System.Windows.Input;
using ControlzEx.Theming;
using dev_flow.Commands;
using dev_flow.Enums;
using dev_flow.Properties;
using dev_flow.ViewModels.Shared;

namespace dev_flow.ViewModels;

public class SettingsPageViewModel : ViewModelBase
{
    public ICommand ThemeChangedCommand { get; }
    public static bool IsDarkTheme => Settings.Default.Theme == ThemeEnum.DarkTheme;

    public SettingsPageViewModel()
    {
        ThemeChangedCommand = new RelayCommand(OnThemeChanged);
    }

    private void OnThemeChanged()
    {
        var currentThemeName = ThemeManager.Current.DetectTheme()?.Name;

        if (currentThemeName != null)
        {
            if (currentThemeName == "Light.Steel")
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Steel");
                Settings.Default.Theme = ThemeEnum.DarkTheme;
            }
            else
            {
                ThemeManager.Current.ChangeTheme(Application.Current, "Light.Steel");
                Settings.Default.Theme = ThemeEnum.LightTheme;
            }

            Settings.Default.Save();
        }
    }
}