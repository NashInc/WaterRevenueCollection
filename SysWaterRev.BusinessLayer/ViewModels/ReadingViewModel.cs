using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class ReadingViewModel : BaseViewModel
    {
        private string employeeFullName;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ReadingId { get; set; }

        [Required]
        [Display(Name = "Latitude")]
        [DataType(DataType.Text)]
        public double Latitude { get; set; }

        [Required]
        [Display(Name = "Longitude")]
        [DataType(DataType.Text)]
        public double Longitude { get; set; }

        [Required]
        [Display(Name = "Reading Value")]
        [DataType(DataType.Text)]
        public int ReadingValue { get; set; }

        [Required]
        [Display(Name = "Confirmed?")]
        [DataType(DataType.Text)]
        public bool? IsConfirmed { get; set; }

        [Required]
        [Display(Name = "Confirmed By")]
        [DataType(DataType.Text)]
        public string ConfirmedBy { get; set; }

        [Required]
        [Display(Name = "Correction")]
        [DataType(DataType.Text)]
        public int CorrectionValue { get; set; }

        [Required]
        [Display(Name = "Corrected By")]
        [DataType(DataType.Text)]
        public string CorrectedBy { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid MeterId { get; set; }

        public Guid CustomerId { get; set; }

        [Display(Name = "Read By")]
        [DataType(DataType.Text)]
        public string EmployeeFullName
        {
            get
            {
                return employeeFullName ??
                       string.Format("{0} {1} {2}", EmployeeFirstName, EmployeeMiddleName, EmployeeSurname);
            }
            set { employeeFullName = value; }
        }

        public string EmployeeFirstName { get; set; }

        public string EmployeeMiddleName { get; set; }

        public string EmployeeSurname { get; set; }

        [Display(Name = "Employee.No")]
        [DataType(DataType.Text)]
        public string EmployeeNumber { get; set; }

        [DisplayName("GPS Accuracy")]
        [DataType(DataType.Text)]
        public double Accuracy { get; set; }

        [DisplayName("GPS Altitude")]
        [DataType(DataType.Text)]
        public double Altitude { get; set; }

        [DisplayName("GPS Speed")]
        [DataType(DataType.Text)]
        public double Speed { get; set; }

        [DisplayName("Acquisition Date")]
        [DataType(DataType.Text)]
        public DateTime LocationDateTime { get; set; }

        [DisplayName("GPS Heading")]
        [DataType(DataType.Text)]
        public double Heading { get; set; }

        [DisplayName("Altitude Accuracy")]
        [DataType(DataType.Text)]
        public double AltitudeAccuracy { get; set; }

        [DisplayName("UserName")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [DisplayName("Meter Number")]
        [DataType(DataType.Text)]
        public string MeterNumber { get; set; }

        [DisplayName("Serial Number")]
        [DataType(DataType.Text)]
        public string MeterSerialNumber { get; set; }

        [DisplayName("Previous Reading")]
        [DataType(DataType.Text)]
        public int PreviousReadingValue { get; set; }

        [DisplayName("Previous Corr. Reading")]
        [DataType(DataType.Text)]
        public int PreviousCorrectedReadingValue { get; set; }

        [DisplayName("Orig Units")]
        [DataType(DataType.Text)]
        public int UnitsConsumedWithNoCorrection
        {
            get { return ReadingValue - PreviousReadingValue; }
        }

        [DisplayName("Corr. Units")]
        [DataType(DataType.Text)]
        public int UnitsConsumedWithCorrection
        {
            get { return CorrectionValue - PreviousCorrectedReadingValue; }
        }

        [DisplayName("Previous Reading ID")]
        public Guid? PreviousReadingId { get; set; }

        public decimal TotalBill { get; set; }
    }
}