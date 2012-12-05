using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Kolonist.Web.Infrastructure
{
    public abstract class KolonistApiControllerBase : ApiController
    {
        protected IResourceLinker _resourceLinker;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            var url = controllerContext.Request.RequestUri;
            var baseUri =
                new UriBuilder(
                    url.Scheme,
                    url.Host,
                    url.Port).Uri;

            _resourceLinker = new RouteLinker(baseUri, this);
        }
    }
}