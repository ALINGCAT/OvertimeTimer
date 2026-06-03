using System.Globalization;
using System.Windows.Data;

namespace OvertimeTimer.App.Converters;

public sealed class ColorTextToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorText || string.IsNullOrWhiteSpace(colorText))
        {
            return System.Windows.Media.Brushes.Transparent;
        }

        try
        {
            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorText);
            return new System.Windows.Media.SolidColorBrush(color);
        }
        catch
        {
            return System.Windows.Media.Brushes.Transparent;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
