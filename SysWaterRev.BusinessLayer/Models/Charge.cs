using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Charges")]
    public class Charge : BaseModel
    {
        [Key]
        public Guid ChargeId { get; set; }

        [Required]
        public double StartRange { get; set; }

        [Required]
        public double EndRange { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public Guid ChargeScheduleId { get; set; }

        [ForeignKey("ChargeScheduleId")]
        public virtual ChargeSchedule ChargeSchedule { get; set; }

        public virtual Charge PreviousCharge { get; set; }   
    }
}