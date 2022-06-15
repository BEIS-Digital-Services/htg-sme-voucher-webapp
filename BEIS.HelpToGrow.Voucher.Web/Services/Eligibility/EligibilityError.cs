
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class EligibilityError : Error
    {
        public Characteristic Characteristic { get; }
        public EligibilityErrorCode ErrorCode { get; }

        public EligibilityError(Characteristic characteristic, string description) : base(description)
        {
            Characteristic = characteristic;
            ErrorCode = characteristic.ErrorCode;
        }

        public EligibilityError(EligibilityErrorCode errorCode, string description) : base(description)
        {
            ErrorCode = errorCode;
        }
    }
}