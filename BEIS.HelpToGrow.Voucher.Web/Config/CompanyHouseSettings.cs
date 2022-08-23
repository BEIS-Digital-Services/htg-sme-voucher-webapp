namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class CompanyHouseSettings
    {
        [Required]
        [Url]
        public string CompanyHouseUrl { get; set; }
        [Required]        
        public string CompanyHouseApiKey { get; set; }
        [Required]        
        public int ConnectionTimeOut { get; set; }
    }
}
