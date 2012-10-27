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
            var names = new[] { "Snow", "Desert", "Swamp", "Marsh", "Mountain1", "Mountain2", "Mountain3", "Mountain4", "Oasis", "Grass1", "Grass2", "Grass3" };

            return names.Select((value, index) => new TerrainType
            {
                Id = index + 1,
                Caption = value
            });
        }

        public string Caption { get; set; }

        public int Id { get; set; }
    }
}