using System.Web;
using Autofac;
using EKSurvey.Data;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace EKSurvey.UI.Modules
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => HttpContext.Current.GetOwinContext())
                .As<IOwinContext>()
                .InstancePerRequest();

            builder.Register(x => HttpContext.Current.GetOwinContext().Get<MembershipDbContext>()).AsSelf().InstancePerRequest();
            builder.Register(x => HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>()).AsSelf().InstancePerRequest();
            builder.Register(x => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>()).AsSelf().InstancePerRequest();

        }
    }
}