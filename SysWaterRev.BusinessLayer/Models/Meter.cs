using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Meters")]
    public class Meter : BaseModel
    {
        [Key]
        public Guid MeterId { get; set; }

        [Required]
        public string MeterSerialNumber { get; set; }

        [Required]
        public string MeterNumber { get; set; }

        public Guid? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer OwnerCustomer { get; set; }

        public virtual ICollection<Reading> MeterReadings { get; set; }
    }
}