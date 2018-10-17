using Google.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.Location
{
    public struct CellUpdateTime
    {
        public S2CellId CellId;

        public DateTime UpdateTime;
    }
}
