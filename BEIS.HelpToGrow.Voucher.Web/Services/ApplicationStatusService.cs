

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public class ApplicationStatusService : IApplicationStatusService
    {
        private readonly HtgVendorSmeDbContext dbContext;
        private readonly ILogger<ApplicationStatusService> logger;

        public ApplicationStatusService(HtgVendorSmeDbContext dbContext, ILogger<ApplicationStatusService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<ApplicationStatus> GetApplicationStatus(string companiesHouseNumber, string fcaNumber)
        {
            return !string.IsNullOrWhiteSpace(companiesHouseNumber)
                ? await GetApplicationStatusForCompaniesHouseNumber(companiesHouseNumber)
                : await GetApplicationStatusForFcaNumber(fcaNumber);
        }

        public async Task<ApplicationStatus> GetApplicationStatusForCompaniesHouseNumber(string companiesHouseNumber)
        {
            var application = await dbContext.enterprises.Where(x => EF.Functions.Like(x.companies_house_no, companiesHouseNumber)).OrderByDescending(x => x.enterprise_created_date).FirstOrDefaultAsync();
            token applicationToken = null;
            if (application != null)
                applicationToken = await dbContext.tokens.FirstOrDefaultAsync(x => x.enterprise_id == application.enterprise_id);

            return getApplicationStatus(application, applicationToken);
        }


        public async Task<ApplicationStatus> GetApplicationStatusForFcaNumber(string fcaNumber)
        {
            var application = await dbContext.enterprises.Where(x => EF.Functions.Like(x.fca_no, fcaNumber)).OrderByDescending(x => x.enterprise_created_date).FirstOrDefaultAsync();
            token applicationToken = null;
            if (application != null)
                applicationToken = await dbContext.tokens.FirstOrDefaultAsync(x => x.enterprise_id == application.enterprise_id);

            return getApplicationStatus(application, applicationToken);
        }


        private static ApplicationStatus getApplicationStatus(enterprise application, token token)
        {
            
            if (application == null)
                return ApplicationStatus.NewApplication;

            if(token == null)
            {
                if (!application.applicant_email_verified)
                    return ApplicationStatus.EmailNotVerified;
                if (application.eligibility_status_id == (long)EligibilityStatus.Failed)
                    return ApplicationStatus.Ineligible;
                if (new[]
                    {
                        (long)EligibilityStatus.Eligible,
                        (long)EligibilityStatus.ReviewRequired,
                        (long)EligibilityStatus.Fca
                    }.Contains(application.eligibility_status_id))
                    return ApplicationStatus.Eligible;

                return ApplicationStatus.EmailVerified;
            }
            else
            {
               switch(token.cancellation_status_id )
                {
                    case 1:
                        {
                            return ApplicationStatus.CancelledCannotReApply;
                        }
                    case 2:
                        {
                            return ApplicationStatus.CancelledInFreeTrialCanReApply;
                        }
                    case 3:
                        {
                            return ApplicationStatus.CancelledNotRedeemedCanReApply;
                        }
                    case 4:
                        {
                            return ApplicationStatus.TokenExpired;
                        }
                }

                if (token.token_expiry > DateTime.Now)
                    return ApplicationStatus.ActiveTokenNotRedeemed;
                else
                {
                    // this is unintuative, but upon redeeeming both redemption and reconciliation status are set to pending.
                    if (token.redemption_status_id == (long)RedemptionStatus.PendingRedemption)
                        return ApplicationStatus.ActiveTokenRedeemed;
                    if (token.reconciliation_status_id == (long)ReconciliationStatus.Reconciled)
                        return ApplicationStatus.TokenReconciled;
                }

                
            }
            return ApplicationStatus.ActiveTokenNotRedeemed;            
        }
    }




    
}
