using Beis.HelpToGrow.Voucher.Web.common;

namespace Beis.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class PhoneNumberViewModel
    {
        [Required(ErrorMessage = "Enter your business telephone number. This could be a landline or mobile")]
        [PhoneNumber(ErrorMessage = "Enter your business telephone number. This could be a landline or mobile")]
        public string PhoneNumber { get; set; }
    }
}
