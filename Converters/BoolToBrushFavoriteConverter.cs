using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace dev_flow.Converters;

public class BoolToBrushFavoriteConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool IsFavorite && IsFavorite)
        {
            return Brushes.Yellow;
        }

        return Brushes.LightYellow;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null!;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}