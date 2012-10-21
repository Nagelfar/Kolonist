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
        public double[][] uvs { get; set; }
        public int[] colors { get; set; }
        public double[] normals { get; set; }


        private static int IndexForVertex(int x, int y, WorldModel model)
        {
            return x * model.Width + y;
        }
        private static int IndexForTexture(int x, int y, WorldModel model)
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
            var uvs = from x in Enumerable.Range(0, world.Width)
                      from y in Enumerable.Range(0, world.Height)
                      select new double[] { (double)x / world.Width, (double)y / world.Height };

            var indices = from x in Enumerable.Range(0, world.Width - 1)
                          from y in Enumerable.Range(0, world.Height - 1)
                          select new[]{
                              0,
                            //(1<<3) + (1<<1),
                            IndexForVertex(x,y,world),
                            IndexForVertex(x+1,y,world),
                            IndexForVertex(x+1,y+1,world),
                            //0,
                            //IndexForTexture(x,y,world),
                            //IndexForTexture(x+1,y,world),
                            //IndexForTexture(x+1,y+1,world),

                            0,
                            //(1<<3) + (1<<1),
                            IndexForVertex(x+1,y+1,world),
                            IndexForVertex(x,y+1,world),
                            IndexForVertex(x,y,world),
                            //0,
                            //IndexForTexture(x+1,y+1,world),
                            //IndexForTexture(x,y+1,world),
                            //IndexForTexture(x,y,world)
                          };



            return new MapModel
            {
                vertices = vertices.SelectMany(x => x).ToArray(),
                faces = indices.SelectMany(x => x).ToArray(),
                materials = new int[]{0},
                colors = new int[0],
                normals = new double[0],
                uvs = new[]{uvs.SelectMany(x => x).ToArray()},
            };
        }

        private class Triangle
        {
            private MapModel _model;
            private int _cIndex;
            private int _bIndex;
            private int _aIndex;

            public Triangle(MapModel model, int aIndex, int bIndex, int cIndex)
            {
                _model = model;
                _aIndex = aIndex;
                _bIndex = bIndex;
                _cIndex = cIndex;
            }

            public void CalculateAndWriteVertexNormals()
            {

            }
        }

        private class Vertex
        {
            private MapModel _model;
            private int _zIndex;
            private int _xIndex;
            private int _yIndex;

            public Vertex(MapModel model, int xIndex, int yIndex, int zIndex)
            {
                _model = model;

                _xIndex = xIndex;
                _yIndex = yIndex;
                _zIndex = zIndex;
            }
        }

    }
}