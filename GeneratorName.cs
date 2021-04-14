using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool Validate(int numItems, out string msg)
        {
            bool result = true;
            string errorMessages = "";

            // Validate minLength
            if(this.minLength < 0)
            {
                errorMessages += "Cannot parse minLength\n";
                result = false;
            }

            if(this.minLength == 0)
            {
                errorMessages += "Minimum length cannot be zero\n";
                result = false;
            }

            if(this.minLength >= maxLength)
            {
                errorMessages += "Minimum length cannot be equal or greater than maximum length\n";
                result = false;
            }

            // Validate maxLength
            if (this.maxLength < 0)
            {
                errorMessages += "Cannot parse maxLength\n";
                result = false;
            }

            if(this.maxLength == 0)
            {
                errorMessages += "Maximum length cannot be zero\n";
                result = false;
            }

            if (this.maxLength <= this.minLength)
            {
                errorMessages += "Maximum length cannot be equal or less than minimum length\n";
                result = false;
            }

            // Validate uniqueness and possible unique item count problem
            if(this.uniqueStrings)
            {
                if (maxLength < 8)
                {
                    errorMessages += "Cannot guarantee uniqueness, raise maximum length to 8\n";
                    result = false;
                }
            }

            msg = "GeneratorName: " + errorMessages;
            return result;
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

            if (this.uniqueStrings && numItems > 10000000)
            {
                this.uniqueStrings = false;
                Console.WriteLine("Generating over 10 000 000 names. Request for unique names is ignored.");
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
