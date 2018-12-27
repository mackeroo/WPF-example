using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ContactManagement.Converters
{
    /// <summary>
    /// converts new contact type to Visibility.
    /// If Customer, then Visibility.Visible
    /// If Vendor, then Visibility.Hidden
    /// </summary>
    public class CustomerToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "Customer")
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
                    return "Customer";
                }
                else
                {
                    return "Vendor";
                }
            }
            return null;
        }
    }
}
