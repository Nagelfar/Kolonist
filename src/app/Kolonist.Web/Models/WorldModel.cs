﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class WorldModel
    {
        public static WorldModel Create(int width = 40, int height = 40)
        {
            var data = new int[width][];
            foreach (var w in Enumerable.Range(0, width))
            {
                data[w] = new int[height];
                foreach (var h in Enumerable.Range(0, height))
                    data[w][ h] = w * h;
            }

            return new WorldModel
            {
                Name = string.Format("Map {0} {1}",width,height),
                Width = width,
                Height = height,
                Id = width*height,
                
                ResourceSet = "1"
            };
        }
        public int Width { get; set; }
        public int Height { get; set; }

        public string ResourceSet { get; set; }

        public string Data { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }
}