using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EuRates
{
    public class Periods
    {
        public DateTime EffectiveFrom { set; get; }

        public Dictionary<string, double> Rates { set; get; }
    }
}
