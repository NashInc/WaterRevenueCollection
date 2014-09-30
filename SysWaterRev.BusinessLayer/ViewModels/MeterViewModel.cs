using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class MeterViewModel : BaseViewModel
    {
        private string customerName;
        private string meterNumberWithOwner;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MeterId { get; set; }

        [Required]
        [Display(Name = "Meter Serial Number")]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 8, MinimumLength = 8, ErrorMessage = "Meter Serial Number Must be 8 Characters")]
        public string MeterSerialNumber { get; set; }

        [Required]
        [Display(Name = "Meter Number")]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 8, MinimumLength = 8, ErrorMessage = "Meter Number Must be 8 Characters")]
        public string MeterNumber { get; set; }

        public string MeterNumberWithOwner
        {
            get { return string.Format("{0} : {1}", CustomerName, MeterNumber); }
            set { meterNumberWithOwner = value; }
        }

        public Guid? CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        [DataType(DataType.Text)]
        public string CustomerName
        {
            get { return string.Format("{0} {1} {2}", FirstName, MiddleName, Surname); }
            set { customerName = value; }
        }

        [Display(Name = "Customer Name With Number")]
        [DataType(DataType.Text)]
        public string CustomerNameWithNumber
        {
            get { return string.Format("{0} : {1}", CustomerName, MeterNumber); }
            set { customerName = value; }
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Surname { get; set; }

        public string CustomerNumber { get; set; }

        public int ReadingsForMeter { get; set; }
    }
}