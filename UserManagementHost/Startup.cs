using Owin;
using Thinktecture.IdentityManager;

namespace UserManagementHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var factory = new AspNetIdentityIdentityManagerFactory();
            app.UseIdentityManager(new IdentityManagerConfiguration
            {
                IdentityManagerFactory = factory.Create
            });
        }
    }
}