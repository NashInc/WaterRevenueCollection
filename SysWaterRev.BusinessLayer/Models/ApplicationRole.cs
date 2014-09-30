using Microsoft.AspNet.Identity.EntityFramework;

namespace SysWaterRev.BusinessLayer.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
        }

        public string Description { get; set; }
    }
}