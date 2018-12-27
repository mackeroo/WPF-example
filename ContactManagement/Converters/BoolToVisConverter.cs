using System;
using System.Windows;
using System.Windows.Data;

namespace ContactManagement.Converters
{
    /// <summary>
    /// converts true values to Visible and false values to collapsed
    /// </summary>
    public class BoolToVisConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Visible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return null;
        }
    }

}
