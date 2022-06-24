using Domain = Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;

namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions
{
    public static class IndesserResponseExtensions
    {
        public static IEnumerable<Domain.Characteristic> GetCharacteristics(this Domain.IndesserCompanyResponse indesserCompanyResponse) =>
            indesserCompanyResponse.Characteristics ?? new List<Domain.Characteristic>();

        public static Result<Domain.Characteristic> Characteristic(this Domain.IndesserCompanyResponse indesserCompanyResponse, string name) =>
            !CharacteristicsByCode(indesserCompanyResponse, name).Any()
                ? Result.Fail($"Missing characteristic {name}")
                : Result.Ok(CharacteristicsByCode(indesserCompanyResponse, name).Single());

        private static IEnumerable<Domain.Characteristic> CharacteristicsByCode(Domain.IndesserCompanyResponse indesserCompanyResponse, string name) =>
            indesserCompanyResponse
                .GetCharacteristics()
                .Where(_ => _.Name.Equals(name))
                .ToList();
    }
}