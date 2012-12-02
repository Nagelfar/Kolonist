using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class BuildingModel
    {
        public bool IsAvaliable { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public Guid BuildingTypeId { get; set; }
    }
}