
namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class DuplicateCharacteristicError : Error
    {
        public string Name => _characteristic.Name;
        public string Code => _characteristic.Code;
        private readonly Characteristic _characteristic;

        public DuplicateCharacteristicError(Characteristic characteristic) : base($"Duplicate characteristic {characteristic.Name} ({characteristic.Code})")
        {
            _characteristic = characteristic;
        }
    }
}