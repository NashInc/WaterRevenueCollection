using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Accounts")]
    public class Account : BaseModel
    {
        [Key]
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; }
        //Navigation Properties
        public Guid CustomerId { get; set; }
        public virtual Customer OwnerCustomer { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}