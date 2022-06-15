
using Newtonsoft.Json;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class EligibilityCheckResultService : IEligibilityCheckResultService
    {
        private readonly IEligibilityCheckResultRepository _repository;
        private readonly ILogger<EligibilityCheckResultService> _logger;

        public EligibilityCheckResultService(
            IEligibilityCheckResultRepository repository,
            ILogger<EligibilityCheckResultService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> SaveAsync(Check check, Result<long> indesserCallSavedResult)
        {
            if (indesserCallSavedResult.IsFailed)
            {
                return Result.Fail("Indesser API call result not saved");
            }

            try
            {
                var eligibility = new eligibility_check_result
                {
                    result_datetime = DateTime.Now,
                    call_id = indesserCallSavedResult.Value,
                    passed_check = check.IsEligible,
                    result_object = JsonConvert.SerializeObject(check),
                    spot_check_object = check.ReviewItems.Any()
                        ? JsonConvert.SerializeObject(check.ReviewItems)
                        : null
                };

                await _repository.AddCheckResult(eligibility);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving eligibility check result for Indesser API call: {indesserCallSavedResult.Value}");

                return Result.Fail(ex.Message);
            }
        }
    }
}