using System;
using System.Collections;
using System.Collections.Generic;
using SysWaterRev.BusinessLayer.Models;
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

    public class GetCustomerRequest
    {
        public Guid? CustomerId { get; set; }
    }

    public class GetCustomerResponse
    {
        public Exception  Exception { get; set; }
        public bool  IsSuccess { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
