using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class GeneratorPhone : IGenerator
	{
		private string allowedChars;
		private string anomalyChars;
		private int minValue;
		private int maxValue;
		private bool hasAnomalies;
		private bool isUnique;

		public GeneratorPhone(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;
			this.minValue = int.Parse(minValue);
			this.maxValue = int.Parse(maxValue);
			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
			HashSet<string> alreadyGeneratedStrings = new HashSet<string>();
			List<string> results = new List<string>();
			List<int> itemIndicesWithAnomalies = new List<int>();

			string[] prefixes = { "040", "045", "050" };

			// Get count of items with anomalies and push to indices
			// to list
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
				string newItem = GenerateNumber(prefixes, rng);

				if (itemIndicesWithAnomalies.Contains(currentItemIndex))
				{
					newItem = GenerateAnomaly(newItem, rng);
				}

				if (isUnique)
				{
					if (!alreadyGeneratedStrings.Contains(newItem))
					{
						alreadyGeneratedStrings.Add(newItem);
						results.Add(newItem);
						currentItemIndex++;
					}
				}
				else
				{
					results.Add(newItem);
					currentItemIndex++;
				}
			}

			return results;
		}

		private string GenerateNumber(string[] prefixes, Random rng)
		{
			// Randomize item length
			int len = rng.Next(minValue, minValue);
			char[] itemAsChars = new char[len];

			string prefix = prefixes[rng.Next(0, prefixes.Length)];
			for(int i = 0; i < prefix.Length; i++)
            {
				itemAsChars[i] = prefix[i];
            }

			for (int i = prefix.Length; i < itemAsChars.Length; i++)
			{
				itemAsChars[i] = allowedChars[rng.Next(0, allowedChars.Length)];
			}

			string s = new string(itemAsChars);
			return s;
		}

		private string GenerateAnomaly(string item, Random rng)
        {
			char[] itemAsChars = item.ToCharArray();
			int indexWithAnomaly = rng.Next(0, item.Length);
			itemAsChars[indexWithAnomaly] = anomalyChars[rng.Next(0, anomalyChars.Length)];
			return new string(itemAsChars);
		}
	}
}
