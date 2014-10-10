using System;
using System.ComponentModel.DataAnnotations;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public abstract class BaseViewModel
    {
 
        [Display(Name = "Date Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Creator")]
        [DataType(DataType.Text)]
        public string CreatedBy { get; set; }


        [Display(Name = "Last Editor")]
        [DataType(DataType.Text)]
        public string LastEditedBy { get; set; }

        [Display(Name = "Last Edit")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastEditDate { get; set; }
    }
}