
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification
{
    public interface IVerifyRegisteredAddressUnchanged : IVerify
    {
        IEnumerable<string> Unchanged { get; }
    }
}