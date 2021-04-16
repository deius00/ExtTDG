using System;
using System.Collections.Generic;
using ExtTDG.Data;

namespace ExtTDG
{
    public class GeneratorAddress : IGenerator
    {
		private string allowedChars;
		private string anomalyChars;
		private int minValue;
		private int maxValue;
		private bool hasAnomalies;
		private bool isUnique;
		private bool isMinValueOk;
		private bool isMaxValueOk;

		public GeneratorAddress(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
			this.allowedChars = allowedChars;
			this.anomalyChars = anomalyChars;

			this.minValue = 0;
			this.isMinValueOk = int.TryParse(minValue, out this.minValue);

			this.maxValue = 0;
			this.isMaxValueOk = int.TryParse(maxValue, out this.maxValue);

			this.hasAnomalies = hasAnomalies;
			this.isUnique = isUnique;
		}


		public bool Validate(int numItems, out ValidationResult result)
		{
			bool isValid = true;
			result = new ValidationResult();

			// Validate minLength
			if (!this.isMinValueOk)
			{
				result.messages.Add(ErrorText.kErrParseMinLen);
				isValid = false;
			}

			if (this.minValue >= maxValue)
			{
				result.messages.Add(ErrorText.kErrMinGEMaxLen);
				isValid = false;
			}

			// Validate maxLength
			if (!this.isMaxValueOk)
			{
				result.messages.Add(ErrorText.kErrParseMaxLen);
				isValid = false;
			}

			if (this.maxValue <= this.minValue)
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

			result.isValid = isValid;
			return result.isValid;
		}

		public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
			HashSet<string> alreadyGeneratedStrings = new HashSet<string>();
			List<string> results = new List<string>();

			int currentItemIndex = 0;
			while(results.Count < numItems)
            {
				string newItem = GenerateRandomAddress(Addresses.kAddresses, rng);
				if(hasAnomalies)
                {
					if(anomalyChance < rng.NextDouble())
                    {
						newItem = GenerateAnomaly(newItem, rng);
					}
				}

				if(isUnique)
                {
					if(!alreadyGeneratedStrings.Contains(newItem))
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

		private string GenerateRandomAddress(string[] addresses, Random rng)
        {
			string address = addresses[rng.Next(0, addresses.Length)];

			if(rng.NextDouble() < 0.5f)
            {
				// Add letter
				string kLetters = "ABCDEFGHJKLMN";
				address += " " + kLetters[rng.Next(0, kLetters.Length)].ToString();
            }

			address += " " + rng.Next(1, 60).ToString();

			return address;
        }

		private string GenerateAnomaly(string item, Random rng)
        {
			char[] itemAsChars = item.ToCharArray();
			int anomalyIndex = rng.Next(0, item.Length);
			itemAsChars[anomalyIndex] = this.anomalyChars[rng.Next(0, this.anomalyChars.Length)];
			return new string(itemAsChars);
        }
	}
}
