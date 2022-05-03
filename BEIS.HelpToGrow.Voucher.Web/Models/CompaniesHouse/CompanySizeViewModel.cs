using BEIS.HelpToGrow.Voucher.Web.Common;
using System.ComponentModel.DataAnnotations;

namespace BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse
{
    public class CompanySizeViewModel
    {
        [Required]
        [NotDefault]
        [RegularExpression("[0-9]*$")]
        public int EmployeeNumbers { get; set; }
    }
}
