using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class MapModel
    {
        public static MapModel Create(int width = 400, int height = 400)
        {
            var data = new int[width][];
            foreach (var w in Enumerable.Range(0, width))
            {
                data[w] = new int[height];
                foreach (var h in Enumerable.Range(0, height))
                    data[w][ h] = w * h;
            }

            return new MapModel
            {
                Name = string.Format("Map {0} {1}",width,height),
                Width = width,
                Height = height,

                Data = data,
                ResourceSet = "1"
            };
        }
        public int Width { get; set; }
        public int Height { get; set; }

        public string ResourceSet { get; set; }

        public int[][] Data { get; set; }

        public string Name { get; set; }
    }
}