
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class MinTradingDuration : AbstractVerification, IVerifyMinTradingDuration
    {
        private int _minTradingDurationInMonths;

        public Characteristic Characteristic => new(nameof(MinTradingDuration), Characteristics.TradingNumberOfMonths, EligibilityErrorCode.CompanyTradingDuration);

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse, int minTradingDurationInMonths)
        {
            _minTradingDurationInMonths = minTradingDurationInMonths;

            var result = indesserCompanyResponse.Characteristic(Characteristic.Code);

            if (result.IsFailed)
            {
                return Verify("missing");
            }

            var characteristic = result.Value;
            return Verify(characteristic.Value);
        }

        protected override Result Verify(string characteristicValue)
        {
            if (!int.TryParse(characteristicValue, out var numberOfMonths))
            {
                return Result.Fail(new EligibilityError(Characteristic, $"Unable to determine trading duration: '{characteristicValue}'"));
            }

            return numberOfMonths < _minTradingDurationInMonths
                ? Result.Fail(new EligibilityError(Characteristic, $"{_minTradingDurationInMonths} months trading history required but only found {numberOfMonths} months"))
                : Result.Ok();
        }
    }
}