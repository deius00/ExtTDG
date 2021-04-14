using System;
using System.Collections.Generic;

namespace ExtTDG
{
    class GeneratorID  : IGenerator
    {
        private string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string anomalyChars = "!#¤%&()?/;.:,_-<>|@£${[]}*";
        private int minLength;
        private int maxLength;
        private bool hasAnomalies;
        private bool uniqueStrings;

        private string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string numbers = "0123456789";

        public GeneratorID(string allowedChars, string anomalyChars,
                string minValue, string maxValue, bool hasAnomalies, bool isUnique)
        {
            SetAllowedAndAnomalyChars(allowedChars, anomalyChars);
            
            try
            {
                this.minLength = Int32.Parse(minValue);
                if (this.minLength <= 0)
                {
                    this.minLength = 3;
                }
            }
            catch (Exception e)
            {
                this.minLength = 3;
            }
            try
            {
                this.maxLength = Int32.Parse(maxValue);
                if (this.maxLength < this.minLength)
                {
                    if (this.minLength >= 6)
                    {
                        this.maxLength = this.minLength;
                    }
                    else
                    {
                        this.maxLength = 6;
                    }
                }
            }
            catch (Exception e)
            {
                this.maxLength = 6;
            }
            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
        } // Constructor

        public bool Validate(int numItems, out string msg)
        {
            msg = "GeneratorID: ";
            return true;
        }


        // Makes sure that allowed and anomaly chars are not conflicting and updates letters and numbers lists.
        public void SetAllowedAndAnomalyChars(string allowedChars, string anomalyChars)
        {
            HashSet<char> allowed = new HashSet<char>(this.allowedChars.ToCharArray());
            HashSet<char> anom = new HashSet<char>(this.anomalyChars.ToCharArray());
            HashSet<char> lett = new HashSet<char>(this.letters.ToCharArray());
            HashSet<char> num = new HashSet<char>(this.numbers.ToCharArray());

            if (!String.IsNullOrWhiteSpace(allowedChars))
            {
                allowed = new HashSet<char>(allowedChars.ToCharArray());
            }
            if (!String.IsNullOrWhiteSpace(anomalyChars))
            {
                anom = new HashSet<char>(anomalyChars.ToCharArray());
            }

            foreach (char c in allowed)
            {
               anom.Remove(c);
            }
            foreach (char c in this.letters)
            {
                if (!allowed.Contains(c))
                    lett.Remove(c);
            }
            foreach (char c in this.numbers)
            {
                if (!allowed.Contains(c))
                    num.Remove(c);
            }

            this.allowedChars = "";
            this.allowedChars = "";
            this.letters = "";
            this.numbers = "";

            foreach (char c in allowed)
                this.allowedChars += c;
            foreach (char c in anom)
                this.anomalyChars += c;
            foreach (char c in lett)
                this.letters += c;
            foreach (char c in num)
                this.numbers += c;


            if (this.anomalyChars.Length == 0)
            {
                this.anomalyChars = "*";
            }
        } // SetAllowedAndAnomalyChars()

        public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
            HashSet<string> alreadyGenerated = new HashSet<string>();
            List<string> results = new List<string>();

            double anomalyProb = anomalyChance;
            if (!this.hasAnomalies)
            {
                anomalyProb = 0;
            }

            // Revert to default allowed chars if trying to generate too much unique IDs with to few chars.
            if (this.uniqueStrings && this.allowedChars.Length < 10 && numItems > 1000000)
            {
                this.allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                this.anomalyChars = "!#¤%&()?/;.:,_-<>|@£${[]}*";
                this.letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                this.numbers = "0123456789";
                Console.WriteLine("Too few allowed characters and large amount of unique IDs. Reverting to default allowed characters.");
            }

            // Increase max length if trying to generate too many unique but short IDs
            if (Math.Log(2 * numItems, this.allowedChars.Length) > this.maxLength)
            {
                this.maxLength = 2 * (int)Math.Log(2 * numItems, this.allowedChars.Length);
                Console.WriteLine("Too much unique but short IDs. Maximum length of the ID increased to " + this.maxLength.ToString());
            }

            // Generate IDs
            string str;
            for (int i = 0; i < numItems; i++)
            {
                if (rng.NextDouble() < anomalyProb)
                {
                    str = GenerateOneID(true, rng);
                }
                else
                {
                    str = GenerateOneID(false, rng);
                }

                if (!this.uniqueStrings || !alreadyGenerated.Contains(str))
                {
                    results.Add(str);
                    if (this.uniqueStrings)
                    {
                        alreadyGenerated.Add(str);
                    }
                }
                else
                {
                    i--;
                }
            } // for

            return results;
        } // Generate()

        // Generates one ID. ID consists of two letters (if allowed and length of the id
        // is at least two) followed by other characters.
        private string GenerateOneID(bool withAnomaly, Random rng)
        {
            string id = "";
            int length = rng.Next(this.minLength, this.maxLength + 1);

            // first letters
            int i = 0;
            if (length > 2 && this.letters.Length > 0)
            {
                id += this.letters[rng.Next(this.letters.Length)];
                i++;
                if (length > 1)
                {
                    id += this.letters[rng.Next(this.letters.Length)];
                    i++;
                }
            }

            // the rest of correct ID
            while (i < length)
            {
                id += this.allowedChars[rng.Next(this.allowedChars.Length)];
                i++;
            }

            // anomalies
            int ind;
            if (withAnomaly)
            {
                int errsAmount = rng.Next(1, id.Length);
                for (int k = 0; k < errsAmount; k++)
                {
                    ind = rng.Next(id.Length);
                    id = id.Remove(ind, 1);
                    id = id.Insert(ind, this.anomalyChars[rng.Next(this.anomalyChars.Length)].ToString());
                }
            }

            return id;
        } // GenerateOneID()
    }
}
