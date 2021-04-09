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
            
            try
            {
                this.minLength = Int32.Parse(minValue);
                if (this.minLength <= 0 || this.minLength > 15)
                {
                    this.minLength = 1;
                    Console.WriteLine("Unusable minimum length. Using default minimum length of 1.");
                }
            }
            catch (Exception e)
            {
                this.minLength = 1;
                Console.WriteLine("Unusable minimum length. Using default minimum length of 1.");
            }
            try
            {
                this.maxLength = Int32.Parse(maxValue);
                if (this.maxLength < this.minLength)
                {
                    this.maxLength = 50;
                    Console.WriteLine("Maximum length less than minimum length. Using default maximum length of 50.");
                }
                if (this.maxLength < 15)
                {
                    this.maxLength = 50;
                    Console.WriteLine("Too short maximum length. Using default maximum length of 50.");
                }
            }
            catch (Exception e)
            {
                this.maxLength = 50;
                Console.WriteLine("Unusable maximum length. Using default maximum length of 50.");
            }
            
            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
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
