
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR09 : CheckEligibilityRule
    {
        public override string Name => "Abnormal filing";
        public override string Description => "Company must not have abnormal filing. 3 or more changes of registered name or addresses in the last 12months constitutes abnormal activity.";
        private readonly ILogger<BR09> _logger;
        private readonly IVerifyNoAbnormalFiling _normalAccountFiling;

        public BR09(ILogger<BR09> logger, IVerifyNoAbnormalFiling normalAccountFiling)
        {
            _logger = logger;
            _normalAccountFiling = normalAccountFiling;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _normalAccountFiling.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, abnormal filing status:{@Status}, result:{@Result}",
                nameof(BR09),
                indesserCompanyResponse.Identification?.companyNumber,
                _normalAccountFiling.None,
                result);

            return result;
        }
    }
}