using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ContactManagement.ValidationRules
{
    /// <summary>
    /// Validate phone numbers
    /// Confirms that the textbox value is integer with 10 digits
    /// </summary>
    public class PhoneValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$");
            Match match = regex.Match(value.ToString());
            if (match == null || match == Match.Empty)
                return new ValidationResult(false, "Invalid input format");
            else
                return ValidationResult.ValidResult;

        }
        public string Expression { get; set; }
    }
}
