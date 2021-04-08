using System;
using System.Collections.Generic;

namespace ExtTDG
{
    public class GeneratorInt32 : IGenerator
	{
		// TODO: Janne
		public GeneratorInt32(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
			List<string> results = new List<string>();
			return results;
		}
	}
}
