using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace dev_flow.Managers.Navigation;

public class NavigationManager
{
    public event NavigatedEventHandler Navigated;

    public event NavigationFailedEventHandler NavigationFailed;

    private Frame _frame;

    public Frame Frame
    {
        get
        {
            if (_frame != null) return _frame;
            _frame = new Frame() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            RegisterFrameEvents();

            return _frame;
        }
        set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }
    }

    public void GoForward() => Frame.GoForward();
    public void GoBack() => Frame.GoBack();
    public bool CanGoBack => Frame.CanGoBack;

    public bool CanGoForward => Frame.CanGoForward;


    public bool Navigate(Uri sourcePageUri, object extraData = null)
    {
        return Frame.CurrentSource != sourcePageUri && Frame.Navigate(sourcePageUri, extraData);
    }

    public bool Navigate(Type sourceType)
    {
        return Frame.NavigationService?.Content?.GetType() != sourceType &&
               Frame.Navigate(Activator.CreateInstance(sourceType));
    }

    private void RegisterFrameEvents()
    {
        if (_frame == null) return;
        _frame.Navigated += Frame_Navigated;
        _frame.NavigationFailed += Frame_NavigationFailed;
    }

    private void UnregisterFrameEvents()
    {
        if (_frame == null) return;
        _frame.Navigated -= Frame_Navigated;
        _frame.NavigationFailed -= Frame_NavigationFailed;
    }

    private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) =>
        NavigationFailed?.Invoke(sender, e);

    private void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);
}