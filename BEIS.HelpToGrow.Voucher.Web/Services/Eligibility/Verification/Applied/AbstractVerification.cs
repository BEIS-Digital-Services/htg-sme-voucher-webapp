

namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied
{
    public abstract class AbstractVerification
    {
        protected virtual Result Verify(IndesserCompanyResponse indesserCompanyResponse, Characteristic characteristic)
        {
            var characteristics =
                indesserCompanyResponse.GetCharacteristics()
                    .Where(_ =>
                        !string.IsNullOrWhiteSpace(_.Name) &&
                        _.Name.Equals(characteristic.Code)).ToList();

            return characteristics.Any() switch
            {
                false => Result.Fail(new MissingCharacteristicError(characteristic)),

                _ => characteristics.Count > 1
                    ? Result.Fail(new DuplicateCharacteristicError(characteristic))
                    : Verify(characteristics.Single().Value)
            };
        }

        protected abstract Result Verify(string value);
    }
}