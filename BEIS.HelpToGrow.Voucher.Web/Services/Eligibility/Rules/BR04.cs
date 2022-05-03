using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules
{
    public class BR04 : CheckEligibilityRule
    {
        public override string Name => $"Company’s number of employees is between {_employeeCount.EmployeeCountMin} and {_employeeCount.EmployeeCountMax}";
        public override string Description => "The company’s number of employees must qualify them as an SME (small to medium sized enterprise)";

        private readonly ILogger<BR04> _logger;
        private readonly IVerifyEmployeeCount _employeeCount;

        public BR04(ILogger<BR04> logger, IVerifyEmployeeCount employeeCount)
        {
            _logger = logger;
            _employeeCount = employeeCount;
        }

        public override Result Check(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var result = _employeeCount.Verify(indesserCompanyResponse, userVoucherDto);

            _logger.LogDebug(
                "rule:{@RuleNumber}, company:{@CompanyNumber}, expected employee count:{@Min}-{@Max}, result:{@Result}",
                nameof(BR04),
                indesserCompanyResponse.Identification?.companyNumber,
                _employeeCount.EmployeeCountMin,
                _employeeCount.EmployeeCountMax,
                result);

            return result;
        }
    }
}