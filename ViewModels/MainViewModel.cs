using dev_flow.Commands;
using dev_flow.ViewModels.Shared;
using System.Windows.Input;
using dev_flow.Enums;
using dev_flow.Managers;
using dev_flow.Properties;

namespace dev_flow.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _selectedPage;

    public ViewModelBase SelectedPage
    {
        get => _selectedPage;
        set
        {
            _selectedPage = value;
            OnPropertyChanged(nameof(SelectedPage));
        }
    }
    
    public bool IsLightTheme => Settings.Default.Theme == ThemeEnum.LightTheme;

    public HomePageViewModel HomePageViewModel { get; }
    public SettingsPageViewModel SettingsPageViewModel { get; }
    public ThemeManager ThemeManager { get; }

    // Commands
    public ICommand NavigateToPageCommand { get; }
    public ICommand ThemeChangedCommand { get; }

    public MainViewModel()
    {
        HomePageViewModel = new HomePageViewModel();
        SettingsPageViewModel = new SettingsPageViewModel();
        ThemeManager = new ThemeManager();

        NavigateToPageCommand = new ParameterRelayCommand<ViewModelBase>(NavigateToPage);
        ThemeChangedCommand = new RelayCommand(SetSelectedTheme);

        // Set the initial page
        SelectedPage = HomePageViewModel;
        // Set the initial theme
        InitialiseTheme();
    }

    private void SetSelectedTheme()
    {
        ThemeManager.SetTheme(Settings.Default.Theme == ThemeEnum.LightTheme
            ? ThemeEnum.DarkTheme
            : ThemeEnum.LightTheme);
    }

    private void InitialiseTheme()
    {
        ThemeManager.SetTheme(Settings.Default.Theme == ThemeEnum.LightTheme
            ? ThemeEnum.LightTheme
            : ThemeEnum.DarkTheme);
    }

    private void NavigateToPage(ViewModelBase pageViewModel)
    {
        SelectedPage = pageViewModel;
    }
}