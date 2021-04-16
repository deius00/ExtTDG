using ExtTDG.Data;
using System;
using System.Collections.Generic;
using System.Numerics;

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
        bool isMinLengthOk;
        bool isMaxLengthOk;

        public GeneratorString(string allowedChars, string anomalyChars,
                string minValue, string maxValue, bool hasAnomalies, bool isUnique)
        {
            this.allowedChars = allowedChars;
            this.anomalyChars = anomalyChars;

            this.isMinLengthOk = int.TryParse(minValue, out this.minLength);
            this.isMaxLengthOk = int.TryParse(maxValue, out this.maxLength);

            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
        }


        public bool Validate(int numItems, out ValidationResult result)
        {
            bool isValid = true;
            result = new ValidationResult();

            if (allowedChars == null || allowedChars.Length == 0)
            {
                result.messages.Add(ErrorText.kErrAllowedCharsEmpty);
                isValid = false;
            }

            if (anomalyChars == null || anomalyChars.Length == 0)
            {
                result.messages.Add(ErrorText.kErrAnomalyCharsEmpty);
                isValid = false;
            }

            if(isValid)
            {
                if(!CheckCharactersIntersection(this.allowedChars, this.anomalyChars))
                {
                    result.messages.Add(ErrorText.kErrSharedCharacters);
                    isValid = false;
                }
            }

            // Validate min length
            if (!isMinLengthOk)
            {
                result.messages.Add(ErrorText.kErrParseMinLen);
                isValid = false;
            }

            // Validate max length
            if (!isMaxLengthOk)
            {
                result.messages.Add(ErrorText.kErrParseMaxLen);
                isValid = false;
            }

            // Validate min/max swap
            if (isValid)
            {
                if (this.minLength >= this.maxLength)
                {
                    result.messages.Add(ErrorText.kErrMinGEMaxLen);
                    isValid = false;
                }

                if (this.maxLength <= this.minLength)
                {
                    result.messages.Add(ErrorText.kErrMaxLEMinLen);
                    isValid = false;
                }
            }

            string uniqueAllowedChars = GetUniqueAllowedChars(this.allowedChars);
            BigInteger numUniqueCharacters = new BigInteger(uniqueAllowedChars.Length);
            BigInteger numPossibilities = System.Numerics.BigInteger.Pow(numUniqueCharacters, this.maxLength);
            BigInteger biNumItems = new BigInteger(numItems);
            if (biNumItems > numPossibilities)
            {
                result.messages.Add(ErrorText.kErrNoUniqueGuaranteeExpandRange);
                isValid = false;
            }

            result.isValid = isValid;
            return result.isValid;
        }
        private bool CheckCharactersIntersection(string s1, string s2)
        {
            HashSet<char> set1 = new HashSet<char>();
            HashSet<char> set2 = new HashSet<char>();

            foreach (char c in s1)
                set1.Add(c);

            foreach (char c in s2)
                set2.Add(c);

            foreach (char c in set1)
            {
                if (set2.Contains(c))
                {
                    return false;
                }
            }

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
