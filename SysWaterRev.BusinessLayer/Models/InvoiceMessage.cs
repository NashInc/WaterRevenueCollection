using System;
using System.ComponentModel.DataAnnotations;

namespace SysWaterRev.BusinessLayer.Models
{
    public abstract class InvoiceMessage : BaseModel
    {
        protected InvoiceMessage(string discriminator)
        {
            Discriminator = discriminator;
        }
        [Key]
        public Guid InvoiceMessageId { get; set; }
        public string MessageReferenceNumber { get; set; }
        [Required]
        public string Discriminator { get; set; }
    }
}