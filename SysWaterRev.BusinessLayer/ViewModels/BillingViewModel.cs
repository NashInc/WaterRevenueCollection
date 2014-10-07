using System.Collections.Generic;

namespace SysWaterRev.BusinessLayer.ViewModels
{
    public class BillingViewModel
    {
        public Dictionary<CustomerViewModel, Dictionary<MeterViewModel, List<ReadingViewModel>>> CustomerWithMeters { get; set; }
        public List<CustomerViewModel> Customer { get; set; }
        public Dictionary<MeterViewModel,List<ReadingViewModel>> MetersForCustomerWithReadings { get; set; } 
    }
}
