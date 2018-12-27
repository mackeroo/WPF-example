using ContactManagement.Models;
using ContactManagement.ViewModels;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace ContactManagement.ValidationRules
{
    public class VendorCodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string code = value.ToString();

            if (code.Length == 4)
            {
                using (ContactsDatabaseEntities context = new ContactsDatabaseEntities())
                {
                    var vcResult = context.VendorCompanies.FirstOrDefault(vc => vc.Vendor_Code == code);

                    if (vcResult != null) // result returned
                    {
                        return new ValidationResult(false, "Vendor Code Exists.");
                    }
                    else
                    {
                        return ValidationResult.ValidResult;
                    }
                }
            }

            else
            {
                return new ValidationResult(false, "Enter at least " + NewContactVM.iMaxLengthTextbox + " characters");
            }
        }
        public string Expression { get; set; }
    }
}
