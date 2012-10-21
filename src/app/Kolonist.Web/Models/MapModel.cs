using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public enum TerrainType
    {
        Sand,
        Water,
        Grass,
        Mountain,
        Hill
    }

    public class MapModel
    {

        public IEnumerable<double> Heights { get; set; }
        public IEnumerable<string> TerrainTypes { get; set; }


        public int Width { get; set; }
        public int Height { get; set; }

        internal static MapModel Create(WorldModel world)
        {
            var terrainNames = Enum.GetNames(typeof(TerrainType));
            var random = new Random();

            var heights = from x in Enumerable.Range(0, world.Width)
                          from y in Enumerable.Range(0, world.Height)
                          let height = random.NextDouble()
                          let terrain = terrainNames[random.Next(terrainNames.Length)]
                          select new { Height = height, Terrain = terrain };



            return new MapModel
            {
                Heights = heights.Select(x => x.Height).ToArray(),
                TerrainTypes = heights.Select(x => x.Terrain).ToArray(),
                Width = world.Width,
                Height = world.Height
            };
        }
    }
}