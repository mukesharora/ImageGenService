using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ClientConfigurator.Converters
{
    class BusyToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Cursor cursor = Cursors.Arrow;

            try
            {
                bool isBusy = (bool)value;

                if (isBusy)
                {
                    cursor = Cursors.Wait;
                }
            }
            catch (Exception)
            {
                // NOP, probably a design time exception
            }

            return cursor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
