using Kolonist.Web.Infrastructure;
using LibNoise.Filter;
using LibNoise.Modifier;
using LibNoise.Primitive;
using LibNoise.Transformer;
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

        public int Width { get; set; }
        public int Height { get; set; }

        public Link CompositeTerrainTile { get; set; }

        internal static HeightMapModel Create(WorldModel world)
        {
            var terrainNames = TerrainType.GetTypes()
                .OrderBy(x => x.BaseHeight)
                .ToArray();

            var noiseMap = new LibNoise.Builder.NoiseMap(world.Width, world.Height)
            {
                BorderValue = 0.0f
            };

            var noise = new ImprovedPerlin()
            {
                Quality = LibNoise.NoiseQuality.Best,
                Seed = DateTime.Now.Millisecond
            };
            var noiseBuilder = new LibNoise.Builder.NoiseMapBuilderPlane(0.0f, 3.0f, 0.0f, 3.0f, true)
            {
                SourceModule = new HybridMultiFractal
                {
                    Primitive3D = noise,
                    //Lacunarity = 1.8f,
                    //Frequency =2.0f
                },
                //SourceModule = new Turbulence
                //{
                //    Power = 0.125f,
                //    XDistortModule = noise,
                //    YDistortModule = noise,
                //    ZDistortModule = noise,
                //    SourceModule = new ScaleBias()
                //    {
                //        Scale = 10.0f,
                //        //Bias = 8.0f,
                //        SourceModule = new LibNoise.Modifier.Select(

                //            controlModule: new Pipe()
                //            {
                //                Frequency = 0.5f,
                //                Lacunarity = 0.25f,
                //                Primitive3D = noise
                //            },
                //            leftModule: new ScaleBias
                //            {
                //                Scale = 0.125f,
                //                Bias = -0.75f,
                //                SourceModule = new Billow
                //                {
                //                    Frequency = 2.0f,
                //                    Primitive3D = noise
                //                }
                //            },
                //            rightModule: new RidgedMultiFractal
                //            {
                //                Primitive3D = noise,
                //                Lacunarity = 2.0f,
                //            },
                //            edge: 0.125f,
                //            lower: 0.0f,
                //            upper: 1000.0f
                //        )
                //    }
                //},
                NoiseMap = noiseMap
            };


            noiseBuilder.SetSize(world.Width, world.Height);

            noiseBuilder.Build();

            var heights = noiseMap.Share().Select(x => (double)x).ToList();
            //float max = 0.0f, min = 0.0f;
            //noiseMap.MinMax(out min, out max);

            return new HeightMapModel
            {
                Heights = heights.ToArray(),
                TerrainTypes = heights.Select(x =>
                {
                    var selected = terrainNames.SkipWhile(terrain => terrain.BaseHeight <= x).FirstOrDefault();
                    return selected ?? terrainNames.Last();
                }).Select(x => x.Id).ToArray(),

                Width = world.Width,
                Height = world.Height
            };
        }
    }
}