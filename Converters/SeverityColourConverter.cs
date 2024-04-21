using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using dev_flow.Enums;

namespace dev_flow.Converters;

public class SeverityColourConverter : MarkupExtension, IValueConverter
{
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