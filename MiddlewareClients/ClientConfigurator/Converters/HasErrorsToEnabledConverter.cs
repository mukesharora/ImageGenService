using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClientConfigurator.Converters
{
    public class HasErrorsToEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // values are a boolean if there are any validation errors.
            // Return TRUE (enabled) if all values are false,
            // Return FALSE (disabled), otherwise.
            
            bool isEnabled = true;
            try
            {
                foreach (object value in values)
                {                    
                    bool hasError = (bool)value;
                    if (hasError)
                    {
                        isEnabled = false;
                        break;
                    }                    
                }
            }
            catch (Exception)
            {
                // NOP, probably design time error.
            }
            return isEnabled;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
