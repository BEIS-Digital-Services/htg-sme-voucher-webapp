using FluentResults;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class MissingCharacteristicError : Error
    {
        public string Name => _characteristic.Name;
        public string Code => _characteristic.Code;
        public EligibilityErrorCode ErrorCode => _characteristic.ErrorCode;

        private readonly Characteristic _characteristic;

        public MissingCharacteristicError(Characteristic characteristic) : base($"Missing characteristic {characteristic.Name} ({characteristic.Code})")
        {
            _characteristic = characteristic;
        }
    }
}