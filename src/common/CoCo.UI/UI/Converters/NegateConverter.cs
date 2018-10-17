using System;
using System.Globalization;
using System.Windows.Data;

namespace CoCo.UI.Converters
{
    public class NegateConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool flag) return !flag;
            throw new ArgumentException("Expected boolean argument", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Convert(value, targetType, parameter, culture);
    }
}