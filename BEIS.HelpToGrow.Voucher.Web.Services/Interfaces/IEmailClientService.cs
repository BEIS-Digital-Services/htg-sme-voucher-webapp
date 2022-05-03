using System.Collections.Generic;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Interfaces
{
    public interface IEmailClientService
    {
        Task SendEmailAsync(string emailAddress, string templateId, Dictionary<string, object> personalisation);
    }
}