using System;
using System.Collections.Generic;


namespace ExtTDG
{
	public class GeneratorURL
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

		public List<string> Generate(int numItems, float anomalyChance, Random rng)
		{
			HashSet<string> alreadyGeneratedStrings = new HashSet<string>();
			List<string> results = new List<string>();

			string[] prefixes = new string[1] { "http://www." };
			string[] suffixes = new string[3] { ".net", ".com", ".fi" };

			int currentItemIndex = 0;
			while (results.Count < numItems)
			{
				string prefix = GeneratePrefix(rng, prefixes);
				string body = GenerateRandomBody(this.minValue, this.maxValue,
					allowedChars, anomalyChars,
					hasAnomalies, isUnique, rng);
				string suffix = GenerateSuffix(rng, suffixes);

				string newItem = prefix + body + suffix;
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
	}
}
