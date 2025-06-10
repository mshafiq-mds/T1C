using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Prodata.WebForm.Startup))]
namespace Prodata.WebForm
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
