
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR06 : CheckEligibilityRule
    {
        public override string Name => "Number of financial agreement providers";
        public override string Description => "The company does not have any presence of Gazette data as sourced from various advertised and unadvertised data sources e.g., insolvency";

        private readonly ILogger<BR06> _logger;
        private readonly IVerifyFinancialAgreementProviders _financialAgreementProviders;

        public BR06(ILogger<BR06> logger, IVerifyFinancialAgreementProviders financialAgreementProviders)
        {
            _logger = logger;
            _financialAgreementProviders = financialAgreementProviders;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _financialAgreementProviders.Verify(indesserCompanyResponse);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, min financial agreement providers:{@Status}, result:{@Result}",
                nameof(BR06),
                indesserCompanyResponse.Identification?.companyNumber,
                _financialAgreementProviders.MinFinancialAgreementProviders,
                result);

            return result;
        }
    }
}