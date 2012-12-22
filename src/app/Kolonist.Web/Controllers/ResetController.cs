using Kolonist.Contracts.Events.Buildings;
using Kolonist.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web.Controllers
{
    public class ResetController : KolonistMvcControllerBase
    {
        public ActionResult CreateBuildingTypes()
        {
            Bus.Publish(new BuildingTypeCreated
            {
                Id = new Contracts.Identities.BuildingTypeId(Guid.NewGuid()),
                Name = "Woodworker"
            });

            return Content("Ok");
        }
    }
}