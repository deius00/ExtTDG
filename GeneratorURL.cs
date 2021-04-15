using System;
using System.Collections.Generic;


namespace ExtTDG
{
	public class GeneratorURL : IGenerator
	{
		private string allowedChars;
		private string anomalyChars;
		private int minValue;
		private int maxValue;
		private bool hasAnomalies;
		private bool isUnique;

		public GeneratorURL(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;
			this.minValue = int.Parse(minValue);
			this.maxValue = int.Parse(maxValue);
			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public bool Validate(int numItems, out string msg)
        {
			msg = "GeneratorURL: ";
			return true;
        }

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
			string[] prefixes = new string[1] { "http://www." };
			string[] suffixes = new string[6] { ".com", ".fi", ".net", ".org", ".eu", ".me" };
			HashSet<string> alreadyGeneratedStrings = new HashSet<string>();
			List<string> results = new List<string>();
			List<int> itemIndicesWithAnomalies = new List<int>();

			// Get count of items with anomalies and push to indices
			// to list
			if(hasAnomalies)
            {
				HashSet<int> usedItemIndices = new HashSet<int>();
				int numAnomalies = (int)((float)numItems * anomalyChance);
				while(itemIndicesWithAnomalies.Count < numAnomalies)
                {
					int newIndex = rng.Next(0, numItems);
					if(!usedItemIndices.Contains(newIndex))
                    {
						usedItemIndices.Add(newIndex);
						itemIndicesWithAnomalies.Add(newIndex);
                    }
				}
			}

			int currentItemIndex = 0;
			while (results.Count < numItems)
			{
				string prefix = GeneratePrefix(rng, prefixes);
				string body = GenerateRandomBody(this.minValue, this.maxValue,
					allowedChars, anomalyChars,
					hasAnomalies, isUnique, rng);
				string suffix = GenerateSuffix(rng, suffixes);
				string newItem = prefix + body + suffix;

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

		private string GeneratePrefix(Random rng, string[] prefixes)
		{
			return prefixes[rng.Next(0, prefixes.Length)];
		}

		private string GenerateRandomBody(int minLength, int maxLength, string allowedChars, string anomalyChars, bool hasAnomalies, bool isUnique, Random rng)
		{
			// Randomize item length
			int len = rng.Next(minLength, maxLength);
			char[] itemAsChars = new char[len];
			for (int i = 0; i < itemAsChars.Length; i++)
			{
				itemAsChars[i] = allowedChars[rng.Next(0, allowedChars.Length)];
			}

			string s = new string(itemAsChars);
			return s;
		}

		private string GenerateSuffix(Random rng, string[] suffixes)
		{
			return suffixes[rng.Next(0, suffixes.Length)];
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
