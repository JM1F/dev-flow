using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;

namespace dev_flow.Managers.Navigation;

public class AnimateFrame
{
    public static readonly DependencyProperty FrameNavigationStoryboardProperty
        = DependencyProperty.RegisterAttached(
            "FrameNavigationStoryboard",
            typeof(Storyboard),
            typeof(AnimateFrame),
            new FrameworkPropertyMetadata(null, OnFrameNavigationStoryboardChanged));

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

    public static void SetFrameNavigationStoryboard(DependencyObject control, Storyboard storyboard)
    {
        control.SetValue(FrameNavigationStoryboardProperty, storyboard);
    }

    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static Storyboard GetFrameNavigationStoryboard(DependencyObject control)
    {
        return (Storyboard)control.GetValue(FrameNavigationStoryboardProperty);
    }
}