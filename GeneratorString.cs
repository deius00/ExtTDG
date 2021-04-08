using System;
using System.Collections.Generic;

namespace ExtTDG
{
    public class GeneratorString : IGenerator
    {
        private string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string anomalyChars = "!#¤%&()=?/;.:,_-<>|@£${[]}*";
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
                this.anomalyChars.Replace(this.allowedChars[i].ToString(), "");
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
                this.maxLength = 1;
            }
            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
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

            string str;
            for (int i = 0; i < numItems; i++)
            {
                if (rng.NextDouble() < anomalyProb)
                {
                    str = GenerateOneString(true, rng);
                } else
                {
                    str = GenerateOneString(true, rng);
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
                    str.Remove(ind, 1);
                    str.Insert(ind, this.anomalyChars[rng.Next(this.anomalyChars.Length)].ToString());
                }
            }

            return str;
        }
    }
}
