

namespace Beis.HelpToGrow.Voucher.Web.Models.FCA
{
    public class FCANumberViewModel
    {
        [Required(ErrorMessage = "Select yes if the business has a Financial Conduct Authority number")]
        public string HasFCANumber { get; set; }
    }
}