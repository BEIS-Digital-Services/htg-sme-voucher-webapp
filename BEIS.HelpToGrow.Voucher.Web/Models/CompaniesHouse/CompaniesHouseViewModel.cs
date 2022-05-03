using System.ComponentModel.DataAnnotations;

namespace BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse
{
    public class CompaniesHouseViewModel
    {
        [Required]
        [StringLength(9, ErrorMessage = "Companies House number length must be between {2} and {1}", MinimumLength = 8)]
        public string Number { get; set; }
        [Required]
        public string HasCompaniesHouseNumber { get; set; }
        public string CompanySize { get; set; }
    }
}