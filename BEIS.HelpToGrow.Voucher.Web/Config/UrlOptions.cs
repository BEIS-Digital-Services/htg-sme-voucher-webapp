
namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class UrlOptions
    {
        [Required]
        [Url]
        public string LearningPlatformUrl { get; set; }

        [Required]
        [Url]
        public string EmailVerificationUrl { get; set; }
    }
}