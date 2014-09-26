using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("ChargeSchedules")]
    public class ChargeSchedule : BaseModel
    {
        [Key]
        public Guid ChargeScheduleId { get; set; }

        [Required]
        public string ChargeScheduleName { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string ActivatedBy { get; set; }

        public DateTime? DateActivated { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public virtual ICollection<Charge> Charges { get; set; }
    }
}