using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class ChargeScheduleViewModel : BaseViewModel
    {
        [Key]
        public Guid ChargeScheduleId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Schedule Name")]
        public string ChargeScheduleName { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Activated By")]
        public string ActivatedBy { get; set; }

        [Display(Name = "Charges")]
        [DataType(DataType.Text)]
        public int ChargesUnderSchedule { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Activated")]
        public DateTime? DateActivated { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Effective Date")]
        public DateTime? EffectiveDate { get; set; }

        private string chargeScheduleNameAndDate;

        [DisplayName("Schedule Name")]
        public string ChargeScheduleNameAndDate
        {
            get
            {
                return chargeScheduleNameAndDate ??
                       string.Format("{0} {1}", ChargeScheduleName, EffectiveDate.ToString());
            }
            set { chargeScheduleNameAndDate = value; }
        }
    }
}