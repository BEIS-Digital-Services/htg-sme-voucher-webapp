
namespace Beis.HelpToGrow.Voucher.Web.Models.CompaniesHouse
{
    public class CompanySizeViewModel
    {
        [Required]
        [NotDefault]
        [RegularExpression("[0-9]*$")]
        public int EmployeeNumbers { get; set; }
    }
}
