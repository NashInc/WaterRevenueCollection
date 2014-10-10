using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("InvoiceLineItems")]
    public class InvoiceLineItem : BaseModel
    {
        [Key]
        public Guid InvoiceLineItemId { get; set; }

        public decimal Amount { get; set; }

        public decimal UnitsConsumed { get; set; }

        public decimal UnitPrice { get; set; }
        //Navigation Properties
        public Guid ChargeScheduleId { get; set; }

        [ForeignKey("ChargeScheduleId")]
        public virtual ChargeSchedule ChargeScheduleUseToComputeInvoice { get; set; }

        public Guid ReadingId { get; set; }

        [ForeignKey("ReadingId")]
        public virtual Reading ReadingToInvoice { get; set; }

        public Guid InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}