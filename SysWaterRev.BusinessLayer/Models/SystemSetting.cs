using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("SystemSettings")]
    public class SystemSetting : BaseModel
    {
        [Key]
        public Guid SystemSettingId { get; set; }

        public Guid ChargeScheduleId { get; set; }

        [ForeignKey(name: "ChargeScheduleId")]
        public virtual ChargeSchedule CurrentChargeSchedule { get; set; }

        public string SetBy { get; set; }
    }
}