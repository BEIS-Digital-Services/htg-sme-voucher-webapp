
namespace Beis.HelpToGrow.Voucher.Web.Models.FCA
{
    public class FCAViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string FCAFullRegistrationNumber { get; set; }

        public string GetRegistrationNumber() =>
            !string.IsNullOrWhiteSpace(FCAFullRegistrationNumber)
                ? FCAFullRegistrationNumber.ToUpper().Trim()
                : string.Empty;
    }
}