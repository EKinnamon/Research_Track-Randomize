using System.Data.Entity;
using System.Web;
using Autofac;
using AutoMapper;
using EKSurvey.Core.Services;
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

            builder.Register(c => HttpContext.Current.GetOwinContext().Get<MembershipDbContext>())
                .AsSelf()
                .InstancePerRequest();

            builder.Register(c => HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>())
                .AsSelf()
                .InstancePerRequest();

            builder.Register(c => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>())
                .AsSelf()
                .InstancePerRequest();

            builder.Register(c => new SurveyManager(c.ResolveNamed<DbContext>("surveyDbContext"), c.Resolve<IMapper>()))
                .As<ISurveyManager>()
                .InstancePerRequest();
        }
    }
}