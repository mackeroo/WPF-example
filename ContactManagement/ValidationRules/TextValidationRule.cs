using ContactManagement.ViewModels;
using System.Globalization;
using System.Windows.Controls;

namespace ContactManagement.ValidationRules
{
    /// <summary>
    /// Validate textbox - based on iMaxLengthTextBox
    /// </summary>
    public class TextValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value.ToString().Length < NewContactVM.iMaxLengthTextbox)
                return new ValidationResult(false,  "Enter at least " + NewContactVM.iMaxLengthTextbox + " characters");
            else
                return ValidationResult.ValidResult;

        }
        public string Expression { get; set; }
    }
}
