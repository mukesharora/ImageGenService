using ClientConfigurator.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClientConfigurator.Validation
{
    public class UniquePortValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);            

            try
            {
                string port = value as string;
                ClientConfigWindow window = App.Current.MainWindow as ClientConfigWindow;
                
                if (window != null)
                {
                    foreach (Models.ClientConfigItem item in window.ViewModel.ClientConfigList)
                    {
                        if (item.ImpinjAppReaderPort == port)
                        {
                            result = new ValidationResult(false, this.ErrorMessage); 
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = new ValidationResult(false, this.ErrorMessage);                
            }
            return result;
        }
    }
}
