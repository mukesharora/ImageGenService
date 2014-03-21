using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ClientConfigurator.Converters
{
    class BusyToHitTestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isHitTestVisible = true;

            try
            {
                bool isBusy = (bool)value;

                isHitTestVisible = !isBusy;
            }
            catch (Exception)
            {
                // NOP, probably a design time exception
            }

            return isHitTestVisible;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
