using System;
using System.ComponentModel.DataAnnotations;
using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Middle Name")]
        [DataType(DataType.Text)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Surname")]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "ID No.")]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 8, MinimumLength = 8, ErrorMessage = "ID No. Must be 8 Characters")]
        public string Identification { get; set; }

        public string FullNameAndNumber
        {
            get { return string.Format("{0} {1} {2}: {3}", FirstName, MiddleName, Surname, CustomerNumber); }
        }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Gender")]
        [DataType(DataType.Text)]
        [Phone]
        public Gender UserGender { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Customer No.")]
        [DataType(DataType.Text)]
        public string CustomerNumber { get; set; }

        [Display(Name = "Meters Owned")]
        [DataType(DataType.Text)]
        public int MetersOwned { get; set; }

        [Display(Name = "Total Units Consumed")]
        [DataType(DataType.Text)]
        public int TotalUnitsConsumed { get; set; }

        [Display(Name = "Total Cost Of Units")]
        [DisplayFormat(DataFormatString = "KES {0:#,###0.00}")]
        public double TotalCost { get; set; }

        public double TotalReadings { get; set; }

        [Display(Name = "Total Cost Of Confirmed Units")]
        [DisplayFormat(DataFormatString = "KES {0:#,###0.00}")]
        public double TotalConfirmedReadingsCost { get; set; }

        public double TotalConfirmedReadings { get; set; }

        [Display(Name = "Total Cost Of Corrected Units")]
        [DisplayFormat(DataFormatString = "KES {0:#,###0.00}")]
        public double TotalCostCorrectedReadings { get; set; }

        public double TotalCorrectedReadings { get; set; }

        [Display(Name = "Total Cost Of Corrected and Confirmed Units")]
        [DisplayFormat(DataFormatString = "KES {0:#,###0.00}")]
        public double TotalCostCorrectedAndConfirmed { get; set; }

        public double TotalCorrectedAndConfirmed { get; set; }
    }
}