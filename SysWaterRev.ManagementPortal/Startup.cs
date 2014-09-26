using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SysWaterRev.ManagementPortal.Startup))]
namespace SysWaterRev.ManagementPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
