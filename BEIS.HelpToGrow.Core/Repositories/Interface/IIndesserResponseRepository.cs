using System.Threading.Tasks;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    public interface IIndesserResponseRepository
    {
        Task<indesser_api_call_status> AddIndesserResponse(indesser_api_call_status indesserApiCallStatus);
    }
}