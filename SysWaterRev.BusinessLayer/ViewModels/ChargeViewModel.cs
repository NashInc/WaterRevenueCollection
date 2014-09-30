using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class ChargeViewModel : BaseViewModel
    {
        public Guid ChargeId { get; set; }

        [Required]
        [Display(Name = "Start Range")]
        [DataType(DataType.Text)]
        [Remote("ChargeStartRangeValidation", "Charges", AdditionalFields = "ChargeScheduleId", HttpMethod = "POST")]
        public double StartRange { get; set; }

        [Required]
        [Display(Name = "End Range")]
        [DataType(DataType.Text)]
        [Remote(action: "ChargeEndRangeValidation", controller: "Charges",
            AdditionalFields = "ChargeScheduleId,StartRange", HttpMethod = "POST")]
        public double EndRange { get; set; }

        [Required]
        [Display(Name = "Unit Price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "KES {0:#,###0.00}")]
        [Remote("ChargeUnitValidation", "Charges", AdditionalFields = "ChargeScheduleId,StartRange,EndRange",
            HttpMethod = "POST")]
        public string UnitPrice { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Remote("ChargeScheduleValidation", "Charges", "ChargeScheduleId,StartRange", HttpMethod = "POST")]
        [DataType(DataType.Text)]
        [Display(Name = "Charge Schedule")]
        [Required]
        public Guid ChargeScheduleId { get; set; }

        [Display(Name = "Schedule Name")]
        public string ChargeScheduleName { get; set; }

        [Display(Name = "Effective Date")]
        public DateTime? ChargeScheduleEffectiveDate { get; set; }
    }
}