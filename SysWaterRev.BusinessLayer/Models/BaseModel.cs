using System;
using System.ComponentModel.DataAnnotations;

namespace SysWaterRev.BusinessLayer.Models
{
    public abstract class BaseModel
    {
        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public string LastEditedBy { get; set; }

        public DateTime? LastEditDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}