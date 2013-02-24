using Kolonist.Contracts.Commands;
using Kolonist.Web.Infrastructure;
using Kolonist.Web.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web.Controllers
{
    public class HomeController : KolonistMvcControllerBase
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuildDialog()
        {
            var buildings = new[]{
                new BuildingModel{
                    Caption="Woodworker",
                    Description="A woodworker works on wood",
                    IsAvaliable=true,
                    BuildingTypeId=Guid.NewGuid()
                },
                new BuildingModel{
                    Caption="Forester",
                    Description="A forester creates trees",
                    IsAvaliable=true,
                    BuildingTypeId=Guid.NewGuid()
                },new BuildingModel{
                    Caption="Stoner",
                    Description="A stoner is usually stoned",
                    IsAvaliable=false,
                    BuildingTypeId=Guid.NewGuid()
                }
            };

            return View(buildings);
        }


        [HttpPost]
        public ActionResult Construct(ConstructBuilding construct)
        {
            var id = Guid.Empty;

            var command = ExecuteCommand(construct, x => id = x.Id.Id);

            if(ModelState.IsValid)
                return Json(id.ToString());
                        
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            this.Response.TrySkipIisCustomErrors=true;

            return Json(ModelState.Where(x => x.Value.Errors.Any()));
        }
    }
}

