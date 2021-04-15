using System;
using System.Collections.Generic;
using System.Numerics;

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
		private bool isMinValueOk;
		private bool isMaxValueOk;

		public GeneratorPhone(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			this.isMinValueOk = int.TryParse(minValue, out this.minValue);
			this.isMaxValueOk = int.TryParse(maxValue, out this.maxValue);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public bool Validate(int numItems, out string msg)
		{
			bool result = true;
			string errorMessages = "";

			// Validate minLength
			if (!this.isMinValueOk)
			{
				errorMessages += "Cannot parse minimum length\n";
				result = false;
			}

			if(this.minValue < 7)
			{
				errorMessages += "Minimum length cannot be less than 7\n";
				result = false;
			}

			if (this.minValue > maxValue)
			{
				errorMessages += "Minimum length cannot be equal or greater than maximum length\n";
				result = false;
			}

			// Validate maxLength
			if (!this.isMaxValueOk)
			{
				errorMessages += "Cannot parse minimum length\n";
				result = false;
			}

			if (this.maxValue < this.minValue)
			{
				errorMessages += "Maximum length cannot be equal or less than minimum length\n";
				result = false;
			}

			// Validate allowed characters
			if (this.allowedChars == null || this.allowedChars.Length == 0)
			{
				errorMessages += "Allowed chars empty\n";
				result = false;
			}

			// Validate anomaly characters
			if (this.anomalyChars == null || this.anomalyChars.Length == 0)
			{
				errorMessages += "Anomaly characters empty\n";
				result = false;
			}

			// Validate uniqueness and number count (is unique / not unique)
			if (this.isUnique)
			{
				string uniqueAllowedChars = GetUniqueAllowedChars(this.allowedChars);
				BigInteger numUniqueCharacters = new BigInteger(uniqueAllowedChars.Length);
				BigInteger powResult = System.Numerics.BigInteger.Pow(numUniqueCharacters, this.maxValue);
				int numPossibilitiesCharacterCount = (int)System.Numerics.BigInteger.Log10(powResult);
				int numItemsCharacterCount = (int)Math.Log10(numItems);

				Console.WriteLine(numItemsCharacterCount + " / " + numPossibilitiesCharacterCount);

				if (numItemsCharacterCount >= numPossibilitiesCharacterCount)
				{
					errorMessages += "Cannot guarantee uniqueness, expand min/max range\n";
					result = false;
				}
			}

			msg = "GeneratorPhone: " + errorMessages;
			return result;
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
			int len = rng.Next(minValue, maxValue);
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

		private string GetUniqueAllowedChars(string str)
		{
			HashSet<char> lookUpTable = new HashSet<char>();
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (!lookUpTable.Contains(c))
				{
					lookUpTable.Add(c);
				}
			}

			int index = 0;
			char[] uniqueChars = new char[lookUpTable.Count];
			foreach (char c in lookUpTable)
			{
				uniqueChars[index] = c;
				index++;
			}

			return new string(uniqueChars);
		}
	}
}
