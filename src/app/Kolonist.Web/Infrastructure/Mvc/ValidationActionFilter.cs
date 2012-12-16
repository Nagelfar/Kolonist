using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.ModelBinding;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;


namespace Kolonist.Web.Infrastructure.Mvc
{
    public static class ValidationActionFilter
    {
        public class WebApiActionFilter : System.Web.Http.Filters.ActionFilterAttribute
        {
            public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
            {
                if (!actionContext.ModelState.IsValid)
                {
                    actionContext.Response =
                        actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                }
            }
        }

        public class MvcActionFilter : System.Web.Mvc.ActionFilterAttribute
        {            
            public override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
            {
                var modelState = filterContext.Controller.ViewData.ModelState;
                if (!modelState.IsValid && filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = modelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(error => error.ErrorMessage)),
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }
            }
        }
    }
}