namespace BEIS.HelpToGrow.Voucher.Web.Services.Config
{
    public class EncryptionSettings
    {
        public string VoucherEncryptionSalt { get; set; }
        public int VoucherEncryptionIteration { get; set; }
        public string VoucherEncryptionInitialVector { get; set; }
        public int VoucherEncryptionKeySize { get; set; }
        public string Salt => VoucherEncryptionSalt;
    }
}