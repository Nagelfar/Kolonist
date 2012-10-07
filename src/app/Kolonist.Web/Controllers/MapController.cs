using Kolonist.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kolonist.Web.Controllers
{
    public class MapController : ApiController
    {
        private static readonly MapModel[] MapModels = new[]{
            MapModel.Create()
        };

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return MapModels.Select(x => x.Name);
        }

        // GET api/<controller>/5
        public MapModel Get(string name)
        {
            return MapModels.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}