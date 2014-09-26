using System;
using System.ComponentModel.DataAnnotations;
using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class ApplicationUserCustomerViewModel
    {
        public string UserName { get; set; }

        public int RoleCount { get; set; }

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
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "ID No.")]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 8, MinimumLength = 8, ErrorMessage = "ID No. Must be 8 Characters")]
        public string Identification { get; set; }

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
    }
}