using System;
using System.Collections.Generic;

namespace ExtTDG
{
    public class GeneratorInt32 : IGenerator
	{
		private string allowedChars;
		private string anomalyChars;
		private int minValue;
		private int maxValue;
		private bool hasAnomalies;
		private bool isUnique;

		public GeneratorInt32(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			int.TryParse(minValue, out this.minValue);
			int.TryParse(maxValue, out this.maxValue);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
			HashSet<string> alreadyGeneratedStrings = new HashSet<string>();
			List<string> results = new List<string>();
			List<int> itemIndicesWithAnomalies = new List<int>();

			if (hasAnomalies)
			{
				HashSet<int> usedItemIndices = new HashSet<int>();
				int numAnomalies = (int)((float)numItems * anomalyChance);
				while (itemIndicesWithAnomalies.Count < numAnomalies)
				{
					int newIndex = rng.Next(0, numItems);
					if (!usedItemIndices.Contains(newIndex))
					{
						usedItemIndices.Add(newIndex);
						itemIndicesWithAnomalies.Add(newIndex);
					}
				}
			}

			int currentItemIndex = 0;
			while (results.Count < numItems)
            {
				int newValue = rng.Next(minValue, maxValue + 1);
				string item = newValue.ToString();

				if (itemIndicesWithAnomalies.Contains(currentItemIndex))
				{
					item = GenerateAnomaly(newValue, rng);
				}

				if (isUnique)
                {
					if(!alreadyGeneratedStrings.Contains(item))
                    {
						alreadyGeneratedStrings.Add(item);
						results.Add(item);
						currentItemIndex++;
					}
				}
				else
                {
					results.Add(item);
					currentItemIndex++;
				}
			}

			return results;
		}

		private string GenerateInt32(Random rng)
        {
			char[] numberAsChars = new char[rng.Next(minValue, maxValue + 1)];
			for(int i = 0; i < numberAsChars.Length; i++)
            {
				numberAsChars[i] = allowedChars[rng.Next(0, allowedChars.Length)];
            }

			return new string(numberAsChars);
        }

		private string GenerateAnomaly(int number, Random rng)
        {
			char[] numberAsChars = number.ToString().ToCharArray();
			int anomalyIndex = rng.Next(0, numberAsChars.Length);
			numberAsChars[anomalyIndex] = anomalyChars[rng.Next(0, anomalyChars.Length)];
			return new string(numberAsChars);
        }
	}
}
