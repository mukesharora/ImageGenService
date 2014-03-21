using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientConfigurator.Validation
{
    public class NumericValidationRule : ValidationRule
    {
        public int MinimumValue { get; set; }


        public int MaximumValue { get; set; }


        public string ErrorMessage { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(false, this.ErrorMessage);
            try
            {
                int intValue;


                if (Int32.TryParse(value as string, out intValue))
                {
                    if ((intValue >= MinimumValue) && (intValue <= MaximumValue))
                    {
                        result = new ValidationResult(true, null);
                    }
                }
            }
            catch (Exception ex)
            {
                // nop
            }

            return result;
        }
    }
}
