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

        public ActionResult TerrainTile(int id)
        {
            //return File("~/Content/map/" + id + ".jpg", "image/jpg");
            return File("~/Content/map/ComposedTexture.png", "image/png");
        }

    }
}
