namespace SysWaterRev.BusinessLayer.Models
{
    public class EmailInvoiceMessage : InvoiceMessage
    {
        public EmailInvoiceMessage(string discriminator) : base(discriminator)
        {
        }

        public string EmailMessage { get; set; }

        public string EmailAddress { get; set; }
    }
}