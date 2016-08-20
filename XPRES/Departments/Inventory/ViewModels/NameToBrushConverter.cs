using System;
using System.Drawing;
using System.Windows.Data;

namespace XPRES.Departments.Inventory.ViewModels
{
    public class NameToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                double _quantity = (double)value;
                if (_quantity >= 99)
                { return Brushes.MediumSeaGreen; }
                else
                { return Brushes.IndianRed; }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
