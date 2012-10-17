using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class MapModel
    {

        public IEnumerable<double> vertices { get; set; }
        public int[] faces { get; set; }
        public int[] materials { get; set; }
        public int[] uvs { get; set; }
        public int[] colors { get; set; }
        public int[] normals { get; set; }


        private static int IndexForVertex(int x, int y, WorldModel model)
        {
            return x * model.Width + y;
        }

        internal static MapModel Create(WorldModel world)
        {
            var random = new Random();
            var vertices = from x in Enumerable.Range(0, world.Width)
                           from y in Enumerable.Range(0, world.Height)
                           let height = random.NextDouble()
                           select new double[] { (double)x, (double)y, height };

            var indices = from x in Enumerable.Range(0, world.Width - 1)
                          from y in Enumerable.Range(0, world.Height - 1)
                          select new[]{
                              0,
                            IndexForVertex(x,y,world),
                            IndexForVertex(x+1,y,world),
                            IndexForVertex(x+1,y+1,world),

                            0,
                            IndexForVertex(x+1,y+1,world),
                            IndexForVertex(x,y+1,world),
                            IndexForVertex(x,y,world)
                          };

            return new MapModel
            {
                vertices = vertices.SelectMany(x => x).ToArray(),
                faces = indices.SelectMany(x => x).ToArray(),
                materials = new int[0],
                colors = new int[0],
                normals = new int[0],
                uvs = new int[0],
            };
        }
    }
}