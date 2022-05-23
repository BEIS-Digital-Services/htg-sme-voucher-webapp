namespace BEIS.HelpToGrow.Voucher.Web.Services.Interfaces
{
    public interface INotifyServiceSettings
    {
        string EmailVerificationUrl { get; }
        string NotifyApiKey { get; }
        string VerifyApplicantEmailAddressTemplateId { get; }
        string IssueTokenTemplateId { get; }
    }
}