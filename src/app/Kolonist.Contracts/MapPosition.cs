using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts
{
    public struct MapPosition
    {
        public readonly double Y;
        public readonly double X;
        
        public MapPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

    }
}
