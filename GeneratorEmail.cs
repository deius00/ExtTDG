﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class GeneratorEmail : IGenerator
    {
        private List<string> firstNames = (new FirstNames()).GetFirstNamesList();
        private List<string> lastNames = (new LastNames()).GetLastNamesList();
        private string allowedChars = "abcdefghijklmnopqrstuvwxyz-@.";
        private string anomalyChars = "!#¤%&()?/;:,_<>|£${[]}*";
        private int minLength;
        private int maxLength;
        private bool hasAnomalies;
        private bool uniqueStrings;

        private string letters = "abcdefghijklmnopqrstuvwxyz";

        public GeneratorEmail(string allowedChars, string anomalyChars,
            string minValue, string maxValue, bool hasAnomalies, bool isUnique)
        {
            if (!String.IsNullOrWhiteSpace(allowedChars))
                this.allowedChars = allowedChars;
            this.allowedChars = this.allowedChars.ToLower();
            this.allowedChars = this.allowedChars.Replace("@", "");
            this.allowedChars = this.allowedChars.Replace(".", "");
            if (!String.IsNullOrWhiteSpace(anomalyChars))
                this.anomalyChars = anomalyChars;
            for (int i = 0; i < this.allowedChars.Length; i++)
                this.anomalyChars = this.anomalyChars.Replace(this.allowedChars[i].ToString(), "");
            this.anomalyChars = this.anomalyChars.Replace("@", "");
            this.anomalyChars = this.anomalyChars.Replace(".", "");
            if (this.anomalyChars.Length == 0)
                this.anomalyChars = "!";
            foreach (char c in this.anomalyChars)
                this.letters = this.letters.Replace(c.ToString(), "");
            if (this.letters.Length == 0)
                this.letters = "abcdefghijklmnopqrstuvwxyz";

            try
            {
                this.minLength = Int32.Parse(minValue);
                if (this.minLength <= 0)
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
                if (this.maxLength < 7)
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

        public bool Validate(int numItems, out string msg)
        {
            msg = "GeneratorEmail: ";
            return true;
        }


        public List<string> Generate(int numItems, double anomalyChance, Random rng)
        {
            HashSet<string> alreadyGenerated = new HashSet<string>();
            List<string> results = new List<string>();

            HashSet<char> allowed = new HashSet<char>(this.allowedChars.ToCharArray());

            double anomalyProb = anomalyChance;
            if (!this.hasAnomalies)
                anomalyProb = 0;

            if (this.uniqueStrings && numItems > 10000000)
            {
                this.uniqueStrings = false;
                Console.WriteLine("Generating over 10 000 000 names. Request for unique names is ignored.");
            }

            string name;
            for (int i = 0; i < numItems; i++)
            {
                if (rng.NextDouble() < anomalyProb)
                    name = GenerateOneEmail(true, allowed, rng);
                else
                    name = GenerateOneEmail(false, allowed, rng);

                if (!this.uniqueStrings || !alreadyGenerated.Contains(name))
                {
                    results.Add(name);
                    if (this.uniqueStrings)
                        alreadyGenerated.Add(name);
                }
                else
                    i--;
            }

            return results;
        }

        private string GenerateOneEmail(bool withAnomaly, HashSet<char> allowed, Random rng)
        {
            string[] suffixes = new string[3] { ".net", ".com", ".fi" };
            string email;

            string suff = suffixes[rng.Next(suffixes.Length)];
            email = GenerateNamePart(allowed, this.minLength - suff.Length - 5, this.maxLength - suff.Length - 3, rng);
            email += GenerateDomainPart(allowed, this.minLength - suff.Length - email.Length, 
                this.maxLength - suff.Length - email.Length, rng);
            email += suff;

            if (withAnomaly)
            {
                int ind;
                int numErrs = rng.Next(1, email.Length);
                for (int i = 0; i < numErrs; i++)
                {
                    ind = rng.Next(email.Length);
                    email = email.Remove(ind, 1);
                    email = email.Insert(ind, this.anomalyChars[rng.Next(this.anomalyChars.Length)].ToString());
                }
            }

            return email;
        }

        private string GenerateNamePart(HashSet<char> allowed, int minLen, int maxLen, Random rng)
        {
            List<string> firstNames = (new FirstNames()).GetFirstNamesList();
            List<string> lastNames = (new LastNames()).GetLastNamesList();
            string namePart;
            bool allowedCharsOnly;
            int tries = 0;

            do
            {
                tries++;
                namePart = firstNames[rng.Next(firstNames.Count)].ToLower();
                namePart = namePart.Replace("å", "a");
                namePart = namePart.Replace("ä", "a");
                namePart = namePart.Replace("ö", "o");

                allowedCharsOnly = true;
                foreach (char c in namePart)
                {
                    if (!allowed.Contains(c))
                    {
                        allowedCharsOnly = false;
                        break;
                    }
                }
            } while (tries < 3 && (namePart.Length < minLen || namePart.Length > maxLen || allowedCharsOnly));

            if (tries == 3 && (namePart.Length < minLen || namePart.Length > maxLen || allowedCharsOnly))
            {
                int min = 1;
                if (minLen > 1)
                    min = minLen - 1;
                int len = rng.Next(min, maxLen);

                namePart = "";
                for (int n = 0; n < len; n++)
                    namePart += this.allowedChars[rng.Next(this.allowedChars.Length)];
            }

            if (tries < 3 && namePart.Length < maxLen-10)
            {
                tries = 0;
                string temp;
                do
                {
                    tries++;
                    temp = namePart + "." + lastNames[rng.Next(lastNames.Count)].ToLower();
                    temp = temp.Replace("å", "a");
                    temp = temp.Replace("ä", "a");
                    temp = temp.Replace("ö", "o");

                    allowedCharsOnly = true;
                    foreach (char c in temp)
                    {
                        if (!allowed.Contains(c))
                        {
                            allowedCharsOnly = false;
                            break;
                        }
                    }
                } while (tries < 3 && (temp.Length < minLen || temp.Length > maxLen || allowedCharsOnly));
            }

            return namePart;
        }

        private string GenerateDomainPart(HashSet<char> allowed, int minLen, int maxLen, Random rng)
        {
            string domain = "@" + this.letters[rng.Next(this.letters.Length)];

            int min = 1;
            if (minLen > 1)
                min = minLen - 1;
            int len = rng.Next(min, maxLen-2);

            for (int n = 0; n < len; n++)
                domain += this.allowedChars[rng.Next(this.allowedChars.Length)];

            return domain;
        }
    }
}
