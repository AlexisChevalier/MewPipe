using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MewPipe.Website.Startup))]
namespace MewPipe.Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
