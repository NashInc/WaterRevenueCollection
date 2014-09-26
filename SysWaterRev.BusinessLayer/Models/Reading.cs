using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.Models
{
    [Table("Readings")]
    public class Reading : BaseModel
    {
        [Key]
        public Guid ReadingId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public int ReadingValue { get; set; }

        public bool? IsConfirmed { get; set; }

        public string ConfirmedBy { get; set; }

        public int CorrectionValue { get; set; }

        public string CorrectedBy { get; set; }

        public double Accuracy { get; set; }

        public double Altitude { get; set; }

        public double Speed { get; set; }

        public DateTime LocationDateTime { get; set; }

        public double Heading { get; set; }

        public double AltitudeAccuracy { get; set; }

        //Relationships
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee ReadBy { get; set; }

        public Guid MeterId { get; set; }

        [ForeignKey("MeterId")]
        public virtual Meter MeterRead { get; set; }

        //Referencing its Parent
        public virtual Reading PreviousReading { get; set; }
    }
}