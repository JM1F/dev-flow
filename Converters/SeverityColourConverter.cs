using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using dev_flow.Enums;

namespace dev_flow.Converters;

/// <summary>
/// Converts a KanbanSeverityEnum value to a Brush. If the value is Low, the converter returns a Green brush; if the value is Medium, the converter returns an Orange brush; if the value is High, the converter returns a Red brush; otherwise, it returns a Gray brush.
/// </summary>
public class SeverityColourConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Converts a KanbanSeverityEnum value to a Brush.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is KanbanSeverityEnum severity)
        {
            switch (severity)
            {
                case KanbanSeverityEnum.Low:
                    return Brushes.Green;
                case KanbanSeverityEnum.Medium:
                    return Brushes.Orange;
                case KanbanSeverityEnum.High:
                    return Brushes.Red;
                default:
                    return Brushes.Gray;
            }
        }

        return Brushes.Gray;
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