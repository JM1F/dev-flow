using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace dev_flow.Views;

/// <summary>
/// Code-behind for the GeneralDisplayBoxControl.xaml file.
/// </summary>
public partial class GeneralDisplayBoxControl : UserControl
{
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(GeneralDisplayBoxControl));

    public object Content
    {
        get { return GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty IconKindProperty =
        DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(GeneralDisplayBoxControl),
            new PropertyMetadata(PackIconKind.None));

    public PackIconKind IconKind
    {
        get { return (PackIconKind)GetValue(IconKindProperty); }
        set { SetValue(IconKindProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(GeneralDisplayBoxControl));

    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(GeneralDisplayBoxControl));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public GeneralDisplayBoxControl()
    {
        InitializeComponent();
    }
}