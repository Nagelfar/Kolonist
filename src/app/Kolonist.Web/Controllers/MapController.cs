﻿using Kolonist.Web.Models;
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
        // GET api/<controller>/5
        public MapModel Get(int id)
        {
            var world =  WorldController.MapModels.SingleOrDefault(x => x.Id == id);
            return MapModel.Create(world);
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