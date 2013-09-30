using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MongoBlog.Core;

namespace MongoBlog.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DbConfig.SetupDb();
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if ("lang" == custom) { 
                return LanguageProvider.GetUICurrentLanguageCode(new HttpContextWrapper(context));
            }

            return base.GetVaryByCustomString(context, custom);
        }
    }
}