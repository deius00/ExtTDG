using System;
using System.Collections.Generic;

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

		public bool Validate(int numItems, out string msg)
		{
			// Check minimum length
			//int numPossibleUniques = Math.Log(2 * numItems, )
			//if (this.isUnique && Math.Log(2 * numItems, this.allowedChars.Length) > this.maxValue - 3)
			//{
			//	this.isUnique = false;
			//	Console.WriteLine("Too few available unique phone numbers of given length. Request for uniqueness is ignored.");
			//}

			msg = "GeneratorAddress: ";
			return true;
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
