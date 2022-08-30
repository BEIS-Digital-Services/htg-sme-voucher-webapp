namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class CompanyHouseHealthCheckConfiguration
    {
        [Required]
        public string CompanyNumber { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyStatus { get; set; }
    }
}
