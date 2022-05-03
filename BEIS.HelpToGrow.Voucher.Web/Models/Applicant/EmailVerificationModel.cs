namespace BEIS.HelpToGrow.Voucher.Web.Models.Applicant
{
    public class EmailVerificationModel
    {
        public long EnterpriseId { get; set; }

        public string EmailAddress { get; set; }

        public long ProductId { get; set; }
    }
}