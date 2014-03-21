using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientConfigurator.Validation
{

    // NOT used
    //
    public class RegexValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string Pattern { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(false, this.ErrorMessage);
            try
            {
                string input = value as string;
                if (input != null)
                {
                    Regex regx = new Regex(Pattern);
                    if (regx.IsMatch(input))
                    {
                        result = new ValidationResult(true, null);
                    }
                }
                else
                {
                    result = new ValidationResult(true, null);
                }
            }
            catch (Exception)
            {
                // nop
            }

            return result;
        }

    }
}
