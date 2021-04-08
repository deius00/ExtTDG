using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    interface IGenerator
    {
        List<string> Generate(int numItems, double anomalyChance, Random rng);
    }
}
