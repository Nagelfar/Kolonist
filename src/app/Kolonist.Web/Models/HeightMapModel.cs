using Kolonist.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{


    public class HeightMapModel
    {
        public IEnumerable<double> Heights { get; set; }
        public IEnumerable<string> TerrainTypes { get; set; }

        public IEnumerable<Link> AvailiableTerrainTypes { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        internal static HeightMapModel Create(WorldModel world)
        {
            double heightModificator = 3.0;
            var terrainNames = TerrainType.GetTypes().ToArray();
            var random = new Random();

            var heights = from x in Enumerable.Range(0, world.Width)
                          from y in Enumerable.Range(0, world.Height)
                          let height = random.NextDouble() * heightModificator
                          let terrain = terrainNames[random.Next(terrainNames.Length)]
                          select new { Height = height, Terrain = terrain };



            return new HeightMapModel
            {
                Heights = heights.Select(x => x.Height).ToArray(),
                TerrainTypes = heights.Select(x => x.Terrain.Caption).ToArray(),
                //AvailiableTerrainTypes = terrainNames,
                Width = world.Width,
                Height = world.Height
            };
        }
    }
}