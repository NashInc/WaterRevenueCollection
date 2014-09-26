using System;
using System.ComponentModel.DataAnnotations;
using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        [Key]
        public Guid EmployeeId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        [MaxLength(length: 20, ErrorMessage = "First Name should be less than 20 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        [DataType(DataType.Text)]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Surname")]
        [DataType(DataType.Text)]
        public string Surname { get; set; }

        public string FullNameWithNumber
        {
            get { return string.Format("{0} {1} : {2}", FirstName, Surname, EmployeeNumber); }
        }

        [Required]
        [Display(Name = "Phone No.")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public string Identification { get; set; }

        [Required]
        [Display(Name = "Employee No.")]
        [DataType(DataType.Text)]
        public string EmployeeNumber { get; set; }

        [Required]
        [Display(Name = "Employee Gender")]
        [DataType(DataType.Text)]
        public Gender EmployeeGender { get; set; }

        [Display(Name = "Readings Count")]
        [DataType(DataType.Text)]
        public int ReadingsCount { get; set; }
    }
}