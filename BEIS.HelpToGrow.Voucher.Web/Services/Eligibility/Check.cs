
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class Check
    {
        public bool IsEligible { get; private set; }
        public List<IError> ReviewItems { get; }
        public List<IError> RecordedItems { get; }
        public List<IError> Errors { get; }
        public EligibilityStatus Eligibility
        {
            get
            {
                if (!IsEligible)
                {
                    return EligibilityStatus.Failed;
                }

                return ReviewItems.Any()
                    ? EligibilityStatus.ReviewRequired
                    : EligibilityStatus.Eligible;
            }   
        }

        public Check(
            bool isEligible,
            List<IError> errors,
            List<IError> reviewItems,
            List<IError> recordedItems)
        {
            IsEligible = isEligible;
            ReviewItems = reviewItems;
            RecordedItems = recordedItems;
            Errors = errors;
        }

        public Check Append(Check check)
        {
            IsEligible = IsEligible && check.IsEligible;
            Errors.AddRange(check.Errors);
            ReviewItems.AddRange(check.ReviewItems);
            RecordedItems.AddRange(check.RecordedItems);

            return this;
        }
    }
}