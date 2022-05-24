namespace BEIS.HelpToGrow.Voucher.FunctionApp
{
    public class TokenReminderOptions
    {
        public string TokenRedeemEmailReminder1TemplateId { get; set; }

        public string TokenRedeemEmailReminder2TemplateId { get; set; }

        public string TokenRedeemEmailReminder3TemplateId { get; set; }

        public int TokenRedeemReminder1Days { get; set; }

        public int TokenRedeemReminder2Days { get; set; }

        public int TokenRedeemReminder3Days { get; set; }
    }
}