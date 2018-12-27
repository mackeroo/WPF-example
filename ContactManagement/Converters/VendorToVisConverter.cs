using System;
using System.Windows;
using System.Windows.Data;

namespace ContactManagement.Converters
{
    public class VendorToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "Vendor")
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Visible)
                {
                    return "Vendor";
                }
                else
                {
                    return "Customer";
                }
            }
            return null;
        }
    }
}
