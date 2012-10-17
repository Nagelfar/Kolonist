using Kolonist.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Http.Routing;

namespace Kolonist.Web.Controllers
{
    public class WorldController : ApiController
    {
        public static readonly WorldModel[] MapModels = new[]{
            WorldModel.Create()
        };

        // GET api/<controller>
        public IEnumerable<int> Get()
        {
            return MapModels.Select(x => x.Id);
        }

        // GET api/<controller>/5
        public WorldModel Get(int id)
        {
            var model = MapModels.SingleOrDefault(x => x.Id == id);
            if (model != null)
            {
                model.Data = "/api/Map/" + id;
                //model.Data = this.Url.Link("/api/Map", new { id = id });
            }
            return model;
        }
    }
}

