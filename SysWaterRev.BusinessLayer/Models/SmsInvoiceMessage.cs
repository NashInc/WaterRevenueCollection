namespace SysWaterRev.BusinessLayer.Models
{
    public class SmsInvoiceMessage : InvoiceMessage
    {
        public SmsInvoiceMessage(string discriminator) : base(discriminator)
        {
        }

        public string SmsMessage { get; set; }

        public string PhoneNumber { get; set; }
    }
}