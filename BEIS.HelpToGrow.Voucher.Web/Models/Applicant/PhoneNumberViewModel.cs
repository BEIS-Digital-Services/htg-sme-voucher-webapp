using Beis.HelpToGrow.Voucher.Web.common;

namespace Beis.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class PhoneNumberViewModel
    {
        [Required(ErrorMessage = "Enter your phone number so that we can provide support for using your discount")]
        [PhoneNumber(ErrorMessage = "Enter your phone number so that we can provide support for using your discount")]
        public string PhoneNumber { get; set; }
    }
}
