using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    class GeneratorAddress : IGenerator
    {
		public GeneratorAddress(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{

		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
			return new List<string>();
        }
	}
}
