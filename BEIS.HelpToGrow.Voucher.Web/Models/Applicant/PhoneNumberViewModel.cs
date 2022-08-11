namespace Beis.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class PhoneNumberViewModel
    {
        [Required(ErrorMessage = "Enter your phone number")]
        public string PhoneNumber { get; set; }
    }
}
