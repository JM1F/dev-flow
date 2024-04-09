using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using dev_flow.Interfaces;

namespace dev_flow.Managers.Navigation;

/// <summary>
/// Manages navigation between pages in the application.
/// </summary>
public class NavigationManager
{
    // Event handler for when a navigation has completed.
    public static event NavigatedEventHandler Navigated;

    // Event handler for when a navigation has failed.
    public event NavigationFailedEventHandler NavigationFailed;

    private Frame _frame;

    /// <summary>
    /// Gets or sets the Frame used for navigation.
    /// </summary>
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

    // Navigates to the previous page in the navigation history.
    public void GoBack() => Frame.GoBack();


    private bool _canGoBack;

    // Gets a value that indicates whether there is at least one entry in the back navigation history.
    public bool CanGoBack => Frame.CanGoBack && PagePreventedFromNavigating();

    // Checks if the page is prevented from navigating to and remove it from the stack.
    private bool PagePreventedFromNavigating()
    {
        foreach (var entry in Frame.BackStack)
        {
            if (entry is JournalEntry { Name: "Views/WorkspaceEditorView.xaml" })
            {
                Frame.RemoveBackEntry();
                break;
            }
        }

        return true;
    }

    /// <summary>
    /// Navigates to a page specified by a Uri.
    /// </summary>
    /// <param name="sourcePageUri">The Uri of the page to navigate to.</param>
    /// <param name="extraData">Data to pass to the new page.</param>
    /// <returns>True if navigation is not canceled; otherwise, false.</returns>
    public bool Navigate(Uri sourcePageUri, object extraData = null)
    {
        return Frame.CurrentSource != sourcePageUri && Frame.Navigate(sourcePageUri, extraData);
    }


    /// <summary>
    /// Navigates to a page specified by a Type.
    /// </summary>
    /// <param name="sourceType">The Type of the page to navigate to.</param>
    /// <returns>True if navigation is not canceled; otherwise, false.</returns>
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