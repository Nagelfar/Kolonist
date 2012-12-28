using Kolonist.Web.App_Start;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Infrastructure.Mvc
{
    public static class DocumentSessionActionFilter
    {
        public class MvcActionFilter : System.Web.Mvc.ActionFilterAttribute
        {
            public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
            {
                var controller = filterContext.Controller as KolonistMvcControllerBase;

                if (controller != null)
                {
                    //controller.DocumentSession = MvcApplication.Container.Resolve<IDocumentSession>();
                }
            }
        }
    }
}