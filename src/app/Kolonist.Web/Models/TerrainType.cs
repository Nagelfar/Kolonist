using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class TerrainType
    {
        public static IEnumerable<TerrainType> GetTypes()
        {
            var s2Names = new[] { "Snow", "Desert", "Swamp", "Marsh", "Mountain1", "Mountain2", "Mountain3", "Mountain4", "Oasis", "Grass1", "Grass2", "Grass3" };
            var names = new[] 
            { 
                new TerrainType
                {
                    Caption="Snow",BaseHeight =1.5,HeightVariance=0.2},
                new TerrainType{
                    Caption= "Sand",BaseHeight=0.5,HeightVariance=0.2},
            new TerrainType{
                Caption= "Grass", BaseHeight=1.0, HeightVariance=0.4},
            new TerrainType{
                Caption="Mountain" ,BaseHeight=1.2,HeightVariance=0.3}
            };

            return names.Select((value, index) =>
            {
                value.Id = index + 1;
                return value;
            });
        }

        public string Caption { get; set; }

        public int Id { get; set; }

        public double BaseHeight { get; set; }
        public double HeightVariance { get; set; }
    }
}