using Kolonist.Contracts.Commands;
using Kolonist.Web.Infrastructure;
using Kolonist.Web.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IServiceBus Bus { get; set; }

        [HttpPost]
        public ActionResult Construct(ConstructANewBuilding command)
        {
            if (ModelState.IsValid)
            {
                Bus.Publish(command);
            }

            return Json(new { r = "ok" });
        }
    }
}

