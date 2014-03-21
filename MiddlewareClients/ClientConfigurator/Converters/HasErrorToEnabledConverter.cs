using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClientConfigurator.Converters
{
    class HasErrorToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool isEnabled = true;
            try
            {
                // value = true if has validation error
                isEnabled = (bool)value;
                isEnabled = !isEnabled;
            }
            catch (Exception)
            {
                // NOP, probably design time error.
            }
            return isEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
