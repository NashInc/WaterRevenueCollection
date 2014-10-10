using SysWaterRev.BusinessLayer.ViewModels;

namespace SysWaterRev.BusinessLayer.Services.CustomerService
{
    public class CreateCustomerRequest
    {
        public CreateCustomerRequest(CreateCustomerViewModel customer)
        {
            CustomerViewModel = customer;
        }
        public CreateCustomerViewModel CustomerViewModel { get; set; }
     
    }
}
