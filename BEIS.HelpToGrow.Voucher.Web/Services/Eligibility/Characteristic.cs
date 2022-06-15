namespace Beis.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public readonly struct Characteristic
    {
        public string Name { get; }
        public string Code { get; }
        public EligibilityErrorCode ErrorCode { get; }

        public Characteristic(string name, string code, EligibilityErrorCode errorCode)
        {
            Name = name;
            Code = code;
            ErrorCode = errorCode;
        }
    }
}