using System.ComponentModel.DataAnnotations;

namespace BEIS.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class TitleOrRoleViewModel
    {
        [Required(ErrorMessage = "Enter your job title or role in the business")]
        public string BusinessRole { get; set; }
    }
}