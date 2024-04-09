using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;

namespace dev_flow.Managers.Navigation;

/// <summary>
/// Provides functionality for animating a Frame during navigation.
/// </summary>
public class AnimateFrame
{
    public static readonly DependencyProperty FrameNavigationStoryboardProperty
        = DependencyProperty.RegisterAttached(
            "FrameNavigationStoryboard",
            typeof(Storyboard),
            typeof(AnimateFrame),
            new FrameworkPropertyMetadata(null, OnFrameNavigationStoryboardChanged));

    /// <summary>
    /// Handles changes to the FrameNavigationStoryboard attached property.
    /// </summary>
    private static void OnFrameNavigationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Frame frame && e.OldValue != e.NewValue)
        {
            frame.Navigating -= Frame_Navigating;
            if (e.NewValue is Storyboard)
            {
                frame.Navigating += Frame_Navigating;
            }
        }
    }

    /// <summary>
    /// Handles the Navigating event of a Frame.
    /// </summary>
    private static void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
        if (sender is Frame frame)
        {
            var sb = GetFrameNavigationStoryboard(frame);
            if (sb != null)
            {
                var presenter = frame.FindChild<ContentPresenter>();
                sb.Begin((FrameworkElement)presenter ?? frame);
            }
        }
    }

    /// <summary>
    /// Sets the value of the FrameNavigationStoryboard attached property for a specified DependencyObject.
    /// </summary>
    public static void SetFrameNavigationStoryboard(DependencyObject control, Storyboard storyboard)
    {
        control.SetValue(FrameNavigationStoryboardProperty, storyboard);
    }


    /// <summary>
    /// Gets the value of the FrameNavigationStoryboard attached property for a specified DependencyObject.
    /// </summary>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static Storyboard GetFrameNavigationStoryboard(DependencyObject control)
    {
        return (Storyboard)control.GetValue(FrameNavigationStoryboardProperty);
    }
}