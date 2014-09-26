using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Employees")]
    public class Employee : BaseModel
    {
        [Key]
        public Guid EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Identification { get; set; }

        [Required]
        public string EmployeeNumber { get; set; }

        [Required]
        public Gender EmployeeGender { get; set; }

        public virtual ICollection<Reading> ReadingsMade { get; set; }
    }
}