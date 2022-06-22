
namespace Beis.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class EmailAddressViewModel
    {
        [Required(ErrorMessage = "Enter your work email address")] 
        [Email( ErrorMessage = "Enter your work email address")]  //"Enter an email address in the correct format, like name@example.com") ] // note this message has been handled manually in the view as its not easy to have a different message display in the summary using out the box validation
        public string EmailAddress { get; set; }
    } 
}