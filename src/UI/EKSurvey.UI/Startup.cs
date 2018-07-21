using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EKSurvey.UI.Startup))]
namespace EKSurvey.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
