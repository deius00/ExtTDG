using System;
using System.Collections.Generic;

namespace ExtTDG
{
    public class GeneratorString : IGenerator
    {
        private string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string anomalyChars = "!#¤%&()?/;.:,_-<>|@£${[]}*";
        private int minLength;
        private int maxLength;
        private bool hasAnomalies;
        private bool uniqueStrings;

        public GeneratorString(string allowedChars, string anomalyChars,
                string minValue, string maxValue, bool hasAnomalies, bool isUnique)
        {
            if (!String.IsNullOrWhiteSpace(allowedChars))
            {
                this.allowedChars = allowedChars;
            }
            if (!String.IsNullOrWhiteSpace(anomalyChars))
            {
                this.anomalyChars = anomalyChars;
            }
            for (int i = 0; i < this.allowedChars.Length; i++)
            {
                this.anomalyChars = this.anomalyChars.Replace(this.allowedChars[i].ToString(), "");
            }
            if (this.anomalyChars.Length == 0)
            {
                this.anomalyChars = "*";
            }
            try {
                this.minLength = Int32.Parse(minValue);
                if (this.minLength <= 0)
                {
                    this.minLength = 1;
                }
            } catch (Exception e)
            {
                this.minLength = 1;
            }
            try
            {
                this.maxLength = Int32.Parse(maxValue);
                if (this.maxLength < this.minLength)
                {
                    if (this.minLength >= 10)
                    {
                        this.maxLength = this.minLength;
                    } else
                    {
                        this.maxLength = 10;
                    }
                }
            }
            catch (Exception e)
            {
                this.maxLength = 10;
            }
            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
        }

        public bool Validate(int numItems, out string msg)
        {
            msg = "GeneratorString: ";
            return true;
        }


        public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
            HashSet<string> alreadyGenerated = new HashSet<string>();
            List<string> results = new List<string>();

            double anomalyProb = anomalyChance;
            if (!this.hasAnomalies)
            {
                anomalyProb = 0;
            }

            // Revert to default allowed chars if trying to generate too much unique Strings with to few chars.
            if (this.uniqueStrings && this.allowedChars.Length < 10 && numItems > 1000000)
            {
                this.allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                this.anomalyChars = "!#¤%&()=?/;.:,_-<>|@£${[]}*";
                Console.WriteLine("Too few allowed characters and large amount of unique Strings. Reverting to default allowed characters.");
            }

            // Increase max length if trying to generate too many unique but short Strings
            if (Math.Log(2 * numItems, this.allowedChars.Length) > this.maxLength)
            { 
                this.maxLength = 2 * (int)Math.Log(2 * numItems, this.allowedChars.Length);
                Console.WriteLine("Too much unique but short Strings. Maximum length of the Strings increased to " + this.maxLength.ToString());
            }

            string str;
            for (int i = 0; i < numItems; i++)
            {
                if (rng.NextDouble() < anomalyProb)
                {
                    str = GenerateOneString(true, rng);
                } else
                {
                    str = GenerateOneString(false, rng);
                }

                if (!this.uniqueStrings || !alreadyGenerated.Contains(str))
                {
                    results.Add(str);
                    if (this.uniqueStrings)
                    {
                        alreadyGenerated.Add(str);
                    }
                } else
                {
                    i--;
                }
            }

            return results;
        }

        private string GenerateOneString(bool withAnomaly, Random rng)
        {
            string str = "";
            int ind;

            // construct a string without an anomaly
            int length = rng.Next(this.minLength, this.maxLength + 1);
            for (int i = 0; i < length; i++)
            {
                ind = rng.Next(this.allowedChars.Length);
                str += this.allowedChars[ind];
            }

            if (withAnomaly)
            {
                int errsAmount = rng.Next(1, str.Length);
                for (int k = 0; k < errsAmount; k++)
                {
                    ind = rng.Next(str.Length);
                    str = str.Remove(ind, 1);
                    str = str.Insert(ind, this.anomalyChars[rng.Next(this.anomalyChars.Length)].ToString());
                }
            }

            return str;
        }
    }
}
