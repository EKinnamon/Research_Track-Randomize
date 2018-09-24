using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EKSurvey.UI.Startup))]
namespace EKSurvey.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigurationInjection(app);
            ConfigureAuth(app);
        }
    }
}
