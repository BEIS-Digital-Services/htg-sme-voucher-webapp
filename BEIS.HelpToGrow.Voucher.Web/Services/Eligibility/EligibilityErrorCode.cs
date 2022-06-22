
namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    /// <summary>
    /// These Error Codes correlate to the business rules.
    /// i.e. The EmployeeCount eligibility error code below relates to BR04
    /// </summary>
    public enum EligibilityErrorCode
    {
        NonUnitedKingdomPostcode = 1,
        CompanyTradingDuration = 2,
        CompanyInactive = 3,
        EmployeeCount = 4,
        Gazette = 5,
        FinancialAgreementProviders = 6,
        DirectorDisqualification = 7,
        AccountFiling = 8,
        AbnormalFiling = 9,
        NonUnitedKingdomHoldingCompanyRegistration = 10,
        CompanyAddressChanged = 11,
        MultipleMatches = 12,
        TotalAgreements = 13,
        ProtectFraudScore = 14,
        ScoreCheck = 15,
        MortgagePresent = 16
    }
}