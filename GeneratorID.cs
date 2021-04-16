using ExtTDG.Data;
using System;
using System.Collections.Generic;
using System.Numerics;

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
        private bool isMinLengthOk;
        private bool isMaxLengthOk;

        private string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string numbers = "0123456789";

        public GeneratorID(string allowedChars, string anomalyChars,
                string minValue, string maxValue, bool hasAnomalies, bool isUnique)
        {
            this.allowedChars = allowedChars;
            this.anomalyChars = anomalyChars;

            this.isMinLengthOk = int.TryParse(minValue, out this.minLength);
            this.isMaxLengthOk = int.TryParse(maxValue, out this.maxLength);

            this.hasAnomalies = hasAnomalies;
            this.uniqueStrings = isUnique;
        } // Constructor


        public bool Validate(int numItems, out ValidationResult result)
        {
            bool isValid = true;
            result = new ValidationResult();

            if(allowedChars == null || allowedChars.Length == 0)
            {
                result.messages.Add(ErrorText.kErrAllowedCharsEmpty);
                isValid = false;
            }

            if(anomalyChars == null || anomalyChars.Length == 0)
            {
                result.messages.Add(ErrorText.kErrAnomalyCharsEmpty);
                isValid = false;
            }

            // Validate allowed and anomaly characters
            // so check that these sets do not contain
            // same characters
            if (isValid)
            {
                if (!CheckCharactersIntersection(this.allowedChars, this.anomalyChars))
                {
                    result.messages.Add(ErrorText.kErrSharedCharacters);
                    isValid = false;
                }
            }

            // Validate min length
            if(!isMinLengthOk)
            {
                result.messages.Add(ErrorText.kErrParseMinLen);
                isValid = false;
            }

            // Validate max length
            if(!isMaxLengthOk)
            {
                result.messages.Add(ErrorText.kErrParseMaxLen);
                isValid = false;
            }

            // Validate min/max swap
            if(isValid)
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

            // Validate uniqueness and number count (is unique / not unique)
            if (this.uniqueStrings)
            {
                string uniqueAllowedChars = GetUniqueAllowedChars(this.allowedChars);
                BigInteger numUniqueCharacters = new BigInteger(uniqueAllowedChars.Length);
                BigInteger numPossibilities = System.Numerics.BigInteger.Pow(numUniqueCharacters, this.maxLength);
                int numPossibilitiesCharacterCount = (int)System.Numerics.BigInteger.Log10(numPossibilities);
                int numItemsCharacterCount = (int)Math.Log10(numItems);
                if (numItemsCharacterCount >= numPossibilitiesCharacterCount)
                {
                    result.messages.Add(ErrorText.kErrNoUniqueGuaranteeExpandRange);
                    isValid = false;
                }
            }
            else
            {
                string uniqueAllowedChars = GetUniqueAllowedChars(this.allowedChars);
                BigInteger numUniqueCharacters = new BigInteger(uniqueAllowedChars.Length);
                BigInteger numPossibilities = System.Numerics.BigInteger.Pow(numUniqueCharacters, this.maxLength);
                BigInteger biNumItems = new BigInteger(numItems);
                if (biNumItems > numPossibilities)
                {
                    result.messages.Add(ErrorText.kErrTooManyItems);
                    isValid = false;
                }
            }

            // Modify letters and numbers if valid
            if (isValid)
            {
                // Allowed characters and anomaly characters are guaranteed to have
                // at least length of 1
                HashSet<char> allowedCharacterSet = new HashSet<char>(this.allowedChars.ToCharArray());
                List<char> allowedLetterList = new List<char>();
                List<char> allowedNumberList = new List<char>();

                // Create list of letters that are used
                // in allowed letters
                foreach(char c in this.letters)
                {
                    if(allowedCharacterSet.Contains(c))
                    {
                        allowedLetterList.Add(c);
                    }
                }

                // Create list of numbers that are used
                // in allowed characters
                foreach(char c in this.numbers)
                {
                    if(allowedCharacterSet.Contains(c))
                    {
                        allowedNumberList.Add(c);
                    }
                }

                // Create new lists of allowed letters and numbers
                int letterIndex = 0;
                char[] newAllowedLetters = new char[allowedLetterList.Count];
                foreach(char c in allowedLetterList)
                {
                    newAllowedLetters[letterIndex] = c;
                    letterIndex++;
                }

                char[] newAllowedNumbers = new char[allowedNumberList.Count];
                int numberIndex = 0;
                foreach(char c in allowedNumberList)
                {
                    newAllowedNumbers[numberIndex] = c;
                    numberIndex++;
                }

                this.letters = new string(newAllowedLetters);
                this.numbers = new string(newAllowedNumbers);
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

            foreach(char c in set1)
            {
                if(set2.Contains(c))
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
