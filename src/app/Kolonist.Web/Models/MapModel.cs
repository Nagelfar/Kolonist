using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class MapModel
    {
        public IEnumerable<Tuple<double, double, double>> Vertices { get; set; }
        public int[] Triangles { get; set; }


        private static int IndexForVertex(int x, int y, WorldModel model)
        {
            return x * model.Width + model.Height;
        }

        internal static MapModel Create(WorldModel world)
        {
            var random = new Random();
            var vertices = from x in Enumerable.Range(0, world.Width)
                           from y in Enumerable.Range(0, world.Height)
                           let height = random.NextDouble()
                           select Tuple.Create((double)x, (double)y, height);

            var indices = from x in Enumerable.Range(0, world.Width - 1)
                          from y in Enumerable.Range(0, world.Height - 1)
                          select new[]{
                            IndexForVertex(x,y,world),
                            IndexForVertex(x+1,y,world),
                            IndexForVertex(x+1,y+1,world),

                            IndexForVertex(x+1,y+1,world),
                            IndexForVertex(x,y+1,world),
                            IndexForVertex(x,y,world)
                          };

            return new MapModel
            {
                Vertices = vertices.ToArray(),
                Triangles = indices.SelectMany(x => x).ToArray()
            };
        }
    }
}