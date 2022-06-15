
using Newtonsoft.Json;

namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class IndesserResponseService : IIndesserResponseService
    {
        private readonly IIndesserResponseRepository _repository;
        private readonly ILogger<IndesserResponseService> _logger;

        public IndesserResponseService(
            IIndesserResponseRepository repository,
            ILogger<IndesserResponseService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<long>> SaveAsync(IndesserCompanyResponse indesserCompanyResponse, long enterpriseId)
        {
            try
            {
                var indesserApiCallStatus = new indesser_api_call_status
                {
                    enterprise_id = enterpriseId,
                    call_datetime = DateTime.Now,
                    result = JsonConvert.SerializeObject(indesserCompanyResponse)
                };

                await _repository.AddIndesserResponse(indesserApiCallStatus);

                return Result.Ok(indesserApiCallStatus.call_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Indesser response");

                return Result.Fail(ex.Message);
            }
        }
    }
}