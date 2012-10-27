using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web.Controllers
{
    public class ResourceController : Controller
    {
        //
        // GET: /Resource/

        public ActionResult TerrainType(int id)
        {
            return File("~/Content/map/" + id, "image/png");
        }

    }
}
