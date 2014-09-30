using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using SysWaterRev.API;

[assembly: OwinStartup(typeof (Startup))]

namespace SysWaterRev.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseCors(CorsOptions.AllowAll);
        }
    }
}