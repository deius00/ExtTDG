using ExtTDG.Data;
using System;
using System.Collections.Generic;
using System.Numerics;

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
		private bool minValueOk;
		private bool maxValueOk;

		public GeneratorInt32(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			minValueOk = int.TryParse(minValue, out this.minValue);
			maxValueOk = int.TryParse(maxValue, out this.maxValue);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}

		public bool Validate(int numItems, out ValidationResult result)
		{
			bool isValid = true;
			result = new ValidationResult();

			// Validate minLength
			if (!this.minValueOk)
			{
				result.messages.Add(ErrorText.kErrParseMinLen);
				isValid = false;
			}

			// Validate maxLength
			if (!this.maxValueOk)
			{
				result.messages.Add(ErrorText.kErrParseMaxLen);
				isValid = false;
			}

			// Validate order of limit lenghts
			if (isValid && this.minValue >= maxValue)
			{
				result.messages.Add(ErrorText.kErrMinGEMaxLen);
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
			int possibleNumbers = 0;
			if (isValid)
			{
				int minVal = Math.Min(this.minValue, this.maxValue);
				int maxVal = Math.Max(this.minValue, this.maxValue);
				if (minVal < 0 && maxVal < 0)
				{
					possibleNumbers = Math.Abs(minVal - maxVal);
				}
				else
				{
					possibleNumbers = Math.Abs(maxVal - minVal);
				}
			}


			if (isValid && this.isUnique)
			{
				// Number range must have at least 10 % overhead because of uniqueness
				if (possibleNumbers < (numItems * 1.1))
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
