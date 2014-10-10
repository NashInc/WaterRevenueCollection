using System;
using SysWaterRev.BusinessLayer.Models;

namespace SysWaterRev.BusinessLayer.Services.CustomerService
{
    public class CreateCustomerResult
    {
        public Exception Exception { get; set; }
        public bool IsSuccess { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}