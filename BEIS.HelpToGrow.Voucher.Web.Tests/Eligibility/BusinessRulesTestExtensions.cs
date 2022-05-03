using System.Linq;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public static class BusinessRulesTestExtensions
    {
        public static Characteristic AddCharacteristic(this IndesserCompanyResponse indesserCompanyResponse, string name)
        {
            if (!indesserCompanyResponse.Characteristics.Any(_ => _.Name.Equals(name)))
            {
                indesserCompanyResponse.Characteristics.Add(new Characteristic { Name = name });
            }
            
            return indesserCompanyResponse.Characteristic(name);
        }

        public static void RemoveCharacteristic(this IndesserCompanyResponse indesserCompanyResponse, string name)
        {
            indesserCompanyResponse.Characteristics = indesserCompanyResponse.Characteristics.Where(_ => !_.Name.Equals(name)).ToList();
        }
    }
}