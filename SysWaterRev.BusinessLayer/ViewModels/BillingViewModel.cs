using System.Collections.Generic;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class BillingViewModel
    {
        public string  InvoiceNumber { get; set; }
        public string  PaymentDueDate { get; set; }
        public CustomerViewModel Customer { get; set; }
    }
}
