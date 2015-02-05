using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HovedOppgave.Startup))]
namespace HovedOppgave
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
