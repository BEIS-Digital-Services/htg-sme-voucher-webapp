using Beis.Htg.VendorSme.Database.Models;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Core.Repositories.Interface
{
    public interface IEnterpriseSizeRepository
    {
        Task<enterprise_size> AddEnterpriseSize(enterprise_size enterpriseSize);
        Task<enterprise_size> GetEnterpriseSizeRecord(string size);
        Task<enterprise_size> GetEnterpriseSizeRecordById(long enterprise_size_id);
    }
}