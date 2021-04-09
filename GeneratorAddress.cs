using System;
using System.Collections.Generic;

namespace ExtTDG
{
    class GeneratorAddress : IGenerator
    {
		private string allowedChars;
		private string anomalyChars;
		private int minValue;
		private int maxValue;
		private bool hasAnomalies;
		private bool isUnique;

		public GeneratorAddress(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			this.minValue = 0;
			int.TryParse(minValue, out this.minValue);

			this.maxValue = 0;
			int.TryParse(maxValue, out this.maxValue);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
			List<string> results = new List<string>();
			
			// NOTE: Placeholder data insertion
			for(int i = 0; i < numItems; i++)
            {
				results.Add("Placeholder 1");
            }

			return results;
        }
	}
}
