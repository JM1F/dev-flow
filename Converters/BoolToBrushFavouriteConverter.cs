using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace dev_flow.Converters;

/// <summary>
/// Converts a boolean value to a Brush. If the boolean is true, the converter returns a Yellow brush; otherwise, it returns a LightYellow brush.
/// </summary>
public class BoolToBrushFavouriteConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a Brush.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A Brush that is Yellow if the value is true; otherwise, LightYellow.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFavourite && isFavourite)
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