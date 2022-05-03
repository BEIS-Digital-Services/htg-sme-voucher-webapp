using BEIS.HelpToGrow.Core.Enums;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public interface IApplicationStatusService
    {
        Task<ApplicationStatus> GetApplicationStatus(string companiesHouseNumber, string fcaNumber);
        Task<ApplicationStatus> GetApplicationStatusForCompaniesHouseNumber(string companiesHouseNumber);
        Task<ApplicationStatus> GetApplicationStatusForFcaNumber(string fcaNumber);
    }
}