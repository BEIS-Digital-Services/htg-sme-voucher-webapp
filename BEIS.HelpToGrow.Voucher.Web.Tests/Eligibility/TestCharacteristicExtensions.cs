using Domain = Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public static class TestCharacteristicExtensions
    {
        public static Domain.Characteristic Characteristic(this IndesserCompanyResponse indesserCompanyResponse, string name) =>
            indesserCompanyResponse.GetCharacteristics().Single(_ => _.Name.Equals(name));

        public static void Remove(this IndesserCompanyResponse indesserCompanyResponse, string name) =>
            indesserCompanyResponse.Characteristics =
                indesserCompanyResponse.Characteristics
                    .Where(_ => !_.Name.Equals(name))
                    .ToList();
    }
}