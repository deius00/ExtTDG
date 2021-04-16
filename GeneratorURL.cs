using System;
using System.Numerics;
using System.Collections.Generic;
using ExtTDG.Data;

namespace ExtTDG
{
	public class GeneratorURL : IGenerator
	{
		private string allowedChars;
		private string anomalyChars;
		private int minLength;
		private int maxLength;
		private bool hasAnomalies;
		private bool isUnique;
		private bool isMinLengthOk;
		private bool isMaxLengthOk;

        public GeneratorURL(string allowedChars, string anomalyChars,
				string minLength, string maxLength, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			this.isMinLengthOk = int.TryParse(minLength, out this.minLength);
			this.isMaxLengthOk = int.TryParse(maxLength, out this.maxLength);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}


		public bool Validate(int numItems, out ValidationResult result)
		{
			bool isValid = true;
			result = new ValidationResult();

			// Validate minLength
			if (!this.isMinLengthOk)
			{
				result.messages.Add(ErrorText.kErrParseMinLen);
				isValid = false;
			}

			if (this.minLength >= maxLength)
			{
				result.messages.Add(ErrorText.kErrMinGEMaxLen);
				isValid = false;
			}

			// Validate maxLength
			if (!this.isMaxLengthOk)
			{
				result.messages.Add(ErrorText.kErrParseMaxLen);
				isValid = false;
			}

			if (this.maxLength <= this.minLength)
			{
				result.messages.Add(ErrorText.kErrMaxLEMinLen);
				isValid = false;
			}

			// Validate allowed characters
			if (this.allowedChars == null || this.allowedChars.Length == 0)
			{
				result.messages.Add(ErrorText.kErrAllowedCharsEmpty);
				isValid = false;
			}

			// Validate anomaly characters
			if (this.anomalyChars == null || this.anomalyChars.Length == 0)
			{
				result.messages.Add(ErrorText.kErrAnomalyCharsEmpty);
				isValid = false;
			}

			// Validate uniqueness and number count (is unique / not unique)
			if (this.isUnique)
			{
				string uniqueAllowedChars = GetUniqueAllowedChars(this.allowedChars);
				BigInteger numUniqueCharacters = new BigInteger(uniqueAllowedChars.Length);
				BigInteger powResult = System.Numerics.BigInteger.Pow(numUniqueCharacters, this.maxLength);
				int numPossibilitiesCharacterCount = (int)System.Numerics.BigInteger.Log10(powResult);
				int numItemsCharacterCount = (int)Math.Log10(numItems);
				if (numItemsCharacterCount >= numPossibilitiesCharacterCount)
				{
					result.messages.Add(ErrorText.kErrNoUniqueGuaranteeExpandRange);
					isValid = false;
				}
			}

			result.isValid = isValid;
			return result.isValid;
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
				string body = GenerateRandomBody(this.minLength, this.maxLength,
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

		private string GetUniqueAllowedChars(string str)
        {
			HashSet<char> lookUpTable = new HashSet<char>();
			for(int i = 0; i < str.Length; i++)
            {
				char c = str[i];
				if(!lookUpTable.Contains(c))
                {
					lookUpTable.Add(c);
                }
            }

			int index = 0;
			char[] uniqueChars = new char[lookUpTable.Count];
			foreach(char c in lookUpTable)
            {
				uniqueChars[index] = c;
				index++;
            }

			return new string(uniqueChars);
        }
	}
}
