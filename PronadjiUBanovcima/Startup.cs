using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PronadjiUBanovcima.Startup))]
namespace PronadjiUBanovcima
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
