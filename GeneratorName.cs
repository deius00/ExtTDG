using System;
using System.Collections.Generic;
using ExtTDG.Data;

namespace ExtTDG
{
    public class GeneratorName : IGenerator
    {
		private List<string> firstNames = (new FirstNames()).GetFirstNamesList();
		private List<string> lastNames = (new LastNames()).GetLastNamesList();
		private string allowedChars = "abcdefghijklmnopqrstuvwxyzåäöABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ-'";
		private string anomalyChars = "!#¤%&()?/;.:,_<>|@£${[]}*";
		private int minLength;
		private int maxLength;
		private bool hasAnomalies;
		private bool uniqueStrings;

		public GeneratorName(string allowedChars, string anomalyChars,
				string minValue, string maxValue, bool hasAnomalies, bool isUnique)
		{
            if (!String.IsNullOrWhiteSpace(allowedChars))
                this.allowedChars = allowedChars;
            string temp = this.allowedChars.ToLower();
            this.allowedChars = temp + " " + this.allowedChars.ToUpper();
            if (!String.IsNullOrWhiteSpace(anomalyChars))
                this.anomalyChars = anomalyChars;
            for (int i = 0; i < this.allowedChars.Length; i++)
                this.anomalyChars = this.anomalyChars.Replace(this.allowedChars[i].ToString(), "");
            if (this.anomalyChars.Length == 0)
                this.anomalyChars = "!";

            // Set -1 to minValue and maxValue if parsing is unsuccessful
            // and do bound checking in Validate()
            if (!int.TryParse(minValue, out this.minLength))
            {
                this.minLength = -1;
            }

            if (!int.TryParse(maxValue, out this.maxLength))
            {
                this.maxLength = -1;
            }

            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
        }

        public bool Validate(int numItems, out ValidationResult result)
        {
            bool isValid = true;
            result = new ValidationResult();

            // Validate minLength
            if (this.minLength < 0)
            {
                result.messages.Add(ErrorText.kErrParseMinLen);
                isValid = false;
            }

            if (this.minLength == 0)
            {
                result.messages.Add(ErrorText.kErrMinNoZeroAllowedLen);
                isValid = false;
            }

            // Validate maxLength
            if (this.maxLength < 0)
            {
                result.messages.Add(ErrorText.kErrParseMaxLen);
                isValid = false;
            }
            else if (this.maxLength < 8)
            {
                result.messages.Add(ErrorText.kErrMaxNoLessThanLen + "8");
                isValid = false;
            }

            if (isValid && this.minLength >= maxLength)
            {
                result.messages.Add(ErrorText.kErrMinGEMaxLen);
                isValid = false;
            }

            // Validate uniqueness and possible unique item count problem
            if (isValid && this.uniqueStrings)
            {
                if (numItems > 10000 && numItems <= 100000 && maxLength < 10)
                {
                    result.messages.Add(ErrorText.kErrNoUniqueGuaranteeRaiseToMaxLen + "10");
                    isValid = false;
                }
                else if (numItems > 100000 && numItems <= 1000000 && maxLength < 12)
                {
                    result.messages.Add(ErrorText.kErrNoUniqueGuaranteeRaiseToMaxLen + "12");
                    isValid = false;
                }
                else if (numItems > 1000000 && numItems <= 10000000 && maxLength < 14)
                {
                    result.messages.Add(ErrorText.kErrNoUniqueGuaranteeRaiseToMaxLen + "14");
                    isValid = false;
                }
                else if (numItems > 10000000)
                {
                    result.messages.Add("Cannot guarantee more than 10 000 000 unique names.");
                    isValid = false;
                }
            }

            result.isValid = isValid;
            return result.isValid;
        }


        public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
            HashSet<string> alreadyGenerated = new HashSet<string>();
            List<string> results = new List<string>();
            HashSet<char> allowed = new HashSet<char>(this.allowedChars.ToCharArray());

            double anomalyProb = anomalyChance;
            if (!this.hasAnomalies)
            {
                anomalyProb = 0;
            }

            string name;
            for (int i = 0; i < numItems; i++)
            {
                if (rng.NextDouble() < anomalyProb)
                {
                    name = GenerateOneName(true, allowed, rng);
                }
                else
                {
                    name = GenerateOneName(false, allowed, rng);
                }

                if (!this.uniqueStrings || !alreadyGenerated.Contains(name))
                {
                    results.Add(name);
                    if (this.uniqueStrings)
                    {
                        alreadyGenerated.Add(name);
                    }
                }
                else
                {
                    i--;
                }
            }

            return results;

        }

        private string GenerateOneName(bool withAnomaly, HashSet<char> allowedChars, Random rng)
        {
            string name;
            bool allowedCharsOnly;
            do
            {
                name = this.firstNames[rng.Next(this.firstNames.Count)]
                    + " " + this.lastNames[rng.Next(this.lastNames.Count)];
                allowedCharsOnly = true;
                foreach (char c in name)
                {
                    if (!allowedChars.Contains(c))
                    {
                        allowedCharsOnly = false;
                        break;
                    }
                }
            } while (name.Length < this.minLength || name.Length > this.maxLength || !allowedCharsOnly);

            if (withAnomaly)
            {
                int ind;
                int numErrs = rng.Next(1, name.Length);
                for (int i = 0; i < numErrs; i++)
                {
                    ind = rng.Next(name.Length);
                    name = name.Remove(ind, 1);
                    name = name.Insert(ind, this.anomalyChars[rng.Next(this.anomalyChars.Length)].ToString());
                }
            }

            return name;
        }

	}
}
