using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Customers")]
    public class Customer : BaseModel
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Identification { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public Gender UserGender { get; set; }

        [Required]
        public string CustomerNumber { get; set; }

        public virtual ICollection<Meter> Meters { get; set; }
    }
}