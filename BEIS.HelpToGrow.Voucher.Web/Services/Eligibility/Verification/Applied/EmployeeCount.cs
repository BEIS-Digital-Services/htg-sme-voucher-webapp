
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public class EmployeeCount : IVerifyEmployeeCount
    {
        public int EmployeeCountMin => 5;
        public int EmployeeCountMax => 249;

        public Result Verify(IndesserCompanyResponse indesserCompanyResponse, UserVoucherDto userVoucherDto)
        {
            var dto = userVoucherDto ?? throw new ArgumentNullException(nameof(userVoucherDto));
            if(dto.EmployeeNumbers < EmployeeCountMin || dto.EmployeeNumbers > EmployeeCountMax)
            {
                return Result.Fail(new EligibilityError(EligibilityErrorCode.EmployeeCount, $"Expected between {EmployeeCountMin} and {EmployeeCountMax} employees for company size but found '{dto.EmployeeNumbers}'"));
            }

            var financial = (indesserCompanyResponse.Financials ?? Enumerable.Empty<Financial>()).ToList();

            if (!financial.Any())
            {
                return Result.Ok();
            }

            var financialData = financial.First().FinancialData;

            if (financialData is null)
            {
                return Result.Ok();
            }

            return financialData.NumberofEmployees > 0 && financialData.NumberofEmployees < EmployeeCountMin
                ? Result.Fail(new EligibilityError(EligibilityErrorCode.EmployeeCount, $"Too few employees. Company with {financialData.NumberofEmployees} employees does not qualify"))
                : financialData.NumberofEmployees > EmployeeCountMax
                    ? Result.Fail(new EligibilityError(EligibilityErrorCode.EmployeeCount, $"Too many employees. Company with {financialData.NumberofEmployees} employees does not qualify"))
                    : Result.Ok();
        }
    }
}