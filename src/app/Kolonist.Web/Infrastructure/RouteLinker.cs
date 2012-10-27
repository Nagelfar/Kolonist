using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;


namespace Kolonist.Web.Infrastructure
{
    public class RouteLinker : IResourceLinker
    {
        private Uri baseUri;
        private readonly ApiController ctx;

        public RouteLinker(Uri baseUri, ApiController ctx)
        {
            this.baseUri = baseUri;
            this.ctx = ctx;
        }

        public Uri GetContent(string relativePath)
        {
            return new Uri(baseUri, relativePath);
        }
        public Uri GetUri<T>(Expression<Action<T>> method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var methodCallExp = method.Body as MethodCallExpression;
            if (methodCallExp == null)
            {
                throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "method");
            }

            var routeValues = methodCallExp.Method.GetParameters()
                .ToDictionary(p => p.Name, p => GetValue(methodCallExp, p));

            var controllerName = methodCallExp.Method.ReflectedType.Name
                .ToLowerInvariant().Replace("controller", "");
            routeValues.Add("controller", controllerName);

            var relativeUri = this.ctx.Url.Route("DefaultApi", routeValues);
            return new Uri(this.baseUri, relativeUri);
        }

        private static object GetValue(MethodCallExpression methodCallExp,
            ParameterInfo p)
        {
            var arg = methodCallExp.Arguments[p.Position];
            var lambda = Expression.Lambda(arg);
            return lambda.Compile().DynamicInvoke().ToString();
        }
    }
}