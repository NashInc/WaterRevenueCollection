using System;
using System.ComponentModel.DataAnnotations;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class SystemSettingsViewModel : BaseViewModel
    {
        [Key]
        public Guid SystemSettingId { get; set; }

        public Guid ChargeScheduleId { get; set; }

        [Display(Name = "Set By")]
        [DataType(DataType.Text)]
        public string SetBy { get; set; }

        [Display(Name = "Schedule Name")]
        [DataType(DataType.Text)]
        public string CurrentChargeScheduleName { get; set; }

        [Display(Name = "Schedule Effective Date")]
        [DataType(DataType.DateTime)]
        public DateTime ChargeScheduleEffectiveDate { get; set; }

        [Display(Name = "Schedule Activated Date")]
        [DataType(DataType.DateTime)]
        public DateTime ChargeScheduleActivatedDate { get; set; }

        [Display(Name = "Schedule Created By")]
        [DataType(DataType.Text)]
        public string ChargeScheduleCreatedBy { get; set; }
    }
}