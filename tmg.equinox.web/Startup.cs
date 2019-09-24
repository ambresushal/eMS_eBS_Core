using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tmg.equinox.web.Startup))]
namespace tmg.equinox.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
