using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class MapModel
    {

        public IEnumerable<double> Heights { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        internal static MapModel Create(WorldModel world)
        {
            var random = new Random();
            var heights = from x in Enumerable.Range(0, world.Width)
                          from y in Enumerable.Range(0, world.Height)
                          let height = random.NextDouble()
                          select height;


            return new MapModel
            {
                Heights = heights.ToArray(),
                Width = world.Width,
                Height = world.Height
            };
        }
    }
}