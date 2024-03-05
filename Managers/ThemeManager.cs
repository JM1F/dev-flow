using System;
using System.ComponentModel;
using System.Windows;
using dev_flow.Enums;
using dev_flow.Properties;

namespace dev_flow.Managers;

public class ThemeManager : INotifyPropertyChanged
{
    private ResourceDictionary _currentTheme;

    public ResourceDictionary CurrentTheme
    {
        get => _currentTheme;
        set
        {
            _currentTheme = value;
            OnPropertyChanged(nameof(CurrentTheme));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetTheme(ThemeEnum theme)
    {
        ResourceDictionary newTheme;
        if (theme == ThemeEnum.LightTheme)
        {
            newTheme = new ResourceDictionary()
            {
                Source = new Uri("Assests/Styles/Themes/LightTheme.xaml", UriKind.Relative)
            };
        }
        else
        {
            newTheme = new ResourceDictionary()
            {
                Source = new Uri("Assests/Styles/Themes/DarkTheme.xaml", UriKind.Relative)
            };
        }
        
        Application.Current.Resources.Clear();
        Application.Current.Resources.MergedDictionaries.Add(newTheme);
        CurrentTheme = newTheme;
        
        Settings.Default.Theme = theme;
        Settings.Default.Save();
    }
}