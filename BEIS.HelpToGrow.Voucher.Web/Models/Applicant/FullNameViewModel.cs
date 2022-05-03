using System.ComponentModel.DataAnnotations;

namespace BEIS.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class FullNameViewModel
    {
        [Required(ErrorMessage = "Enter your full name")]

        public string Name { get; set; }
    }
}