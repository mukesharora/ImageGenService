using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ClientConfigurator.Converters
{
    class IsDirtyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = Visibility.Hidden;

            try
            {
                bool isDirty = (bool)value;

                if (isDirty)
                {
                    visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                // NOP, probably a design time exception
            }

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
