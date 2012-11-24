using Kolonist.Web.Infrastructure;
using LibNoise.Filter;
using LibNoise.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{


    public class HeightMapModel
    {
        public IEnumerable<double> Heights { get; set; }
        public IEnumerable<int> TerrainTypes { get; set; }

        public IEnumerable<Link> AvailiableTerrainTypes { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        internal static HeightMapModel Create(WorldModel world)
        {
            //double heightModificator = 3.0;
            var terrainNames = TerrainType.GetTypes()
                .OrderBy(x=>x.BaseHeight)
                .ToArray();

            var noiseMap = new LibNoise.Builder.NoiseMap(world.Width, world.Height);
            var noise = new ImprovedPerlin()
                    {
                        Quality = LibNoise.NoiseQuality.Best
                    };
            var noiseBuilder = new LibNoise.Builder.NoiseMapBuilderPlane(0.0f, 1.0f, 0.0f, 1.0f, false)
            {
                SourceModule = new MultiFractal()
                {
                    Lacunarity = 2.0f,
                    Frequency = 1.0f,
                    Primitive2D = noise,
                    Primitive3D = noise
                },
                NoiseMap = noiseMap,
            };


            noiseBuilder.SetSize(world.Width, world.Height);

            noiseBuilder.Build();

            var heights = noiseMap.Share().Select(x => (double)x).ToList();
            float max = 0.0f, min = 0.0f;
            noiseMap.MinMax(out min, out max);
            
            return new HeightMapModel
            {
                Heights = heights.ToArray(),
                TerrainTypes = heights.Select(x =>
                {
                    var selected = terrainNames.SkipWhile(terrain => terrain.BaseHeight <= x).FirstOrDefault();
                    return selected ?? terrainNames.Last();
                    //if (x <= 0.2)
                    //    return terrainNames[0];
                    //else if (x <= 0.5)
                    //    return terrainNames[1];
                    //else if (x <= 0.8)
                    //    return terrainNames[2];
                    //else
                    //    return terrainNames[3];
                }).Select(x => x.Id).ToArray(),
                //AvailiableTerrainTypes = terrainNames,
                Width = world.Width,
                Height = world.Height
            };
        }
    }
}