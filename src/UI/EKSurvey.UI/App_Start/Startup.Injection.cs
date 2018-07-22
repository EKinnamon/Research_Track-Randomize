using System.Reflection;
using System.Web.Mvc;

using Autofac;
using Autofac.Integration.Mvc;
using EKSurvey.UI.Modules;
using Owin;

namespace EKSurvey.UI
{
    public partial class Startup
    {
        public static void ConfigurationInjection(IAppBuilder app)
        {
            var bindAssembly = Assembly.GetExecutingAssembly();
            var builder = new ContainerBuilder();

            // register autofac modules here
            builder.RegisterModule<ServicesModule>();
            builder.RegisterModule<ApplicationModule>();

            builder.RegisterControllers(bindAssembly);

            var container = builder.Build();
            app.UseAutofacMiddleware(container);

            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);
        }
    }
}