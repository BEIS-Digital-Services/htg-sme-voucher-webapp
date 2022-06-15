
namespace Beis.HelpToGrow.Voucher.Web.Models.CompaniesHouse
{
    public class CompaniesHouseNumberViewModel
    {
        [Required(ErrorMessage = "Companies House number is required")]
        [StringLength(9, ErrorMessage = "Companies House number length must be between {2} and {1}", MinimumLength = 8)]
        public string Number { get; set; }

        public CompanyHouseResponse CompanyHouseResponse { get; set; }

        public string GetNumber() => Number.Trim().ToUpper();
    }
}