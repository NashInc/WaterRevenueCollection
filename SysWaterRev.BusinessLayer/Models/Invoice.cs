using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Invoices")]
    public class Invoice : BaseModel
    {
        [Key]
        public Guid InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal NetAmount { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsSent { get; set; }

        public string SentBy { get; set; }

        public DateTime? DateSent { get; set; }
        //Navigation Properties       
        public virtual ICollection<InvoiceMessage> InvoiceMessages { get; set; }
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account AccountToInvoice { get; set; }
        public virtual ICollection<InvoiceLineItem> InvoiceLineItems { get; set; }
    }
}