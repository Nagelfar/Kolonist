using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web.Controllers
{
    public class ResourceController : Controller
    {        

        public ActionResult CompositeTile(int worldId)
        {
            return File("~/Content/map/ComposedTexture.png", "image/png");
        }
    }
}
