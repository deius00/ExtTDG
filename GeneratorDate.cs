using System;
using System.Collections.Generic;

namespace ExtTDG
{
	public class GeneratorDate
	{
        private int dateFormat = 0; 
        private string anomalyChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!#¤%&()=?;:,_<>|@£${[]}"; 
        private DateTime minDate; 
        private DateTime maxDate; 
        private bool hasAnomalies; 
        private bool uniqueDates;
        
        private bool specifiedRange = false;
        private bool skipUniqueCheck = false;

        public GeneratorDate(string allowedChars, string anomalyChars, 
                string minValue, string maxValue, bool hasAnomalies, bool isUnique) 
        {
            SetDateFormat(allowedChars);
            if (!String.IsNullOrWhiteSpace(anomalyChars))
            {
                this.anomalyChars = anomalyChars;
            }
            SetMinDate(minValue);
            SetMaxDate(maxValue);
            this.hasAnomalies = hasAnomalies;
            this.uniqueDates = isUnique;
        }

        public void SetDateFormat(string formatStr)
        {
            int temp;
            try {
                temp = Int32.Parse(formatStr);
                if (temp == 1 || temp == 2) 
                {
                    this.dateFormat = temp;
                }
            } catch (Exception e)
            {
                Console.WriteLine("Given date format not recognized, default will be used.");
            }
        }

        public void SetMinDate(string minDate)
        {
            try 
            {
                int date = Int32.Parse(minDate);
                int year = date / 10000;
                int month = (date - year*10000) / 100;
                int day = date - year*10000 - month*100;

                this.minDate = new DateTime(year, month, day);  // trhows an exception if not possible
                this.specifiedRange = true;
            } catch (Exception e)
            {
                this.minDate = new DateTime(1000, 1, 1);
            }
        }

        public void SetMaxDate(string maxDate)
        {
            try 
            {
                int date = Int32.Parse(maxDate);
                int year = date / 10000;
                int month = (date - year*10000) / 100;
                int day = date - year*10000 - month*100;

                this.maxDate = new DateTime(year, month, day);  // trhows an exception if not possible
                this.specifiedRange = true;
            } catch (Exception e)
            {
                this.maxDate = new DateTime(9999, 12, 31);
            }
        }

        private void CheckLimitDates()
        {
            if (this.minDate.CompareTo(this.maxDate) > 0)
            {
                this.minDate = new DateTime(1000, 1, 1);
                this.maxDate = new DateTime(9999, 12, 31);
                this.specifiedRange = false;
            }
        }

        private void CheckAmountOfOptionsForUnique(int amount)
        {
            if (amount > 100000)
            {
                this.uniqueDates = false;
            }
            else if (this.maxDate.CompareTo(this.minDate) <= 2*amount)
            {
                this.minDate = new DateTime(1000, 1, 1);
                this.maxDate = new DateTime(9999, 12, 31);
                this.specifiedRange = false;
            }
        }

        public List<string> Generate(int numItems, double anomalyChance, Random rng)
		{
			HashSet<string> alreadyGenerated = new HashSet<string>();
			List<string> results = new List<string>();

            CheckLimitDates();
            CheckAmountOfOptionsForUnique(numItems);
            double anomalyProb = 0;
            if (this.hasAnomalies)
            {
                anomalyProb = anomalyChance;
            }

            int minY = this.minDate.Year;
            int maxY = this.maxDate.Year;
            int minM = this.minDate.Month;
            int maxM = this.maxDate.Month;
            int minD = this.minDate.Day;
            int maxD = this.maxDate.Day;
            int year = 0;
            int month = 0;
            int day = 0;
            string date ="";

            for (int i = 0; i < numItems; i++)
            {
                // generate a viable date
                year = rng.Next(minY, maxY+1);

                if (minY == maxY) {
                    month = rng.Next(minM, maxM+1);
                } else if (year == minY) {
                    month = rng.Next(minM, 12);
                } else if (year == maxY) {
                    month = rng.Next(1, maxM+1);
                } else {
                    month = rng.Next(1, 12);
                }

                if (minY == maxY && minM == maxM) {
                    day = rng.Next(minD, maxD+1);
                } else if (year == minY && month == minM) {
                    day = rng.Next(minD, DateTime.DaysInMonth(year, month)+1);
                } else if (year == maxY && month == maxM) {
                    day = rng.Next(1, maxD+1);
                } else {
                    day = rng.Next(1, DateTime.DaysInMonth(year, month)+1);
                }

                // create anomalies and build date string
                if (rng.NextDouble() < anomalyProb)
                {
                    date = DateWithAnomaly(year, month, day, results, rng);
                }
                else 
                {
                    date = BuildDateString(year.ToString(), month.ToString(), day.ToString(), true, true);
                }

                // add to results
                if (this.skipUniqueCheck || !this.uniqueDates || !alreadyGenerated.Contains(date))
                {
                    results.Add(date);
                    this.skipUniqueCheck = false;
                    if (this.uniqueDates) 
                    {
                        alreadyGenerated.Add(date);
                    }
                } else 
                {
                    i--;
                }
            }

            return results;
		} // Generate()

        private string DateWithAnomaly(int year, int month, int day, List<string> previouslyGenerated, Random rng)
        {
            string date;
            int anomalyType = 1;
            
            // decide the type of anomaly
            if (this.uniqueDates && this.specifiedRange)
            {
                anomalyType = rng.Next(1, 6);
            }
            else if (this.uniqueDates)
            {
                anomalyType = rng.Next(1, 5);
                if (anomalyType == 4)
                {
                    anomalyType = 5;
                }
            }
            else if (this.specifiedRange)
            {
                anomalyType = rng.Next(1, 5);
            }
            else 
            {
                anomalyType = rng.Next(1, 4);
            }

            // create anomaly
            if (anomalyType == 1)
            {
                date = BuildDateString(year.ToString(), month.ToString(), day.ToString(), true, true);
                int numChars = rng.Next(1, 4);
                for (int k = 0; k < numChars; k++)
                {
                    int pos = rng.Next(date.Length);
                    int ac = rng.Next(this.anomalyChars.Length);
                    date = date.Insert(pos, this.anomalyChars.Substring(ac, 1));
                }
            }
            else if (anomalyType == 2)
            {
                date = ImpossibleDate(year, month, day, rng);
            }
            else if (anomalyType == 3)
            {
                date = MissingPart(year, month, day, rng);
            }
            else if (anomalyType == 4)
            {
                date = DateOutOfBounds(year, month, day, rng);
            }
            else // anomalyType == 5
            {
                if (previouslyGenerated.Count == 0)
                {
                    date = BuildDateString(year.ToString(), month.ToString(), day.ToString(), true, true);
                }
                else
                {
                    int ind = rng.Next(previouslyGenerated.Count);
                    date = previouslyGenerated[ind];
                    this.skipUniqueCheck = true;
                }
            }

            return date;
        } // DateWithAnomaly()

        private string ImpossibleDate(int year, int month, int day, Random rng)
        {
            int numErrs = rng.Next(1, 4);
            int pos = 0;
            int err = 0;
            int y = year;
            int m = month;
            int d = day;

            for (int i = 0; i < numErrs; i++) {
                pos = rng.Next(3);
                if (pos == 0) {
                    err = rng.Next(9999);
                    if (err == 0) {
                        y = 0;
                    } else {
                        y = 9999 + err;
                    }
                } 
                else if (pos == 1) {
                    err = rng.Next(30);
                    if (err == 0) {
                        m = 0;
                    } else {
                        m = 12 + err;
                    }
                } 
                else {
                    err = rng.Next(99);
                    if (err == 0) {
                        d = 0;
                    } else {
                        d = 31 + err;
                    }
                }
            }
            return BuildDateString(y.ToString(), m.ToString(), d.ToString(), true, true);
        } // ImpossibleDate()

        private string MissingPart(int year, int month, int day, Random rng)
        {
            int n = -1;
            int m = -1;

            int numErrs = rng.Next(1, 6);

            bool yMissing = false;
            bool mMissing = false;
            bool dMissing = false;
            bool c1Missing = false;
            bool c2Missing = false;

            if (numErrs < 3) {
                n = rng.Next(5);
                if (numErrs == 2) {
                    do {
                        m = rng.Next(5);
                    } while (m == n);
                }
                
                if (n == 0 || m == 0) {
                    yMissing = true;
                }
                if (n == 1 || m == 1) {
                    mMissing = true;
                }
                if (n == 2 || m == 2) {
                    dMissing = true;
                }
                if (n == 3 || m == 3) {
                    c1Missing = true;
                }
                if (n == 4 || m == 4) {
                    c2Missing = true;
                }
            }
            else {
                yMissing = true;
                mMissing = true;
                dMissing = true;
                c1Missing = true;
                c2Missing = true;
                
                if (numErrs < 5) {
                    n = rng.Next(5);
                }
                if (numErrs == 3) {
                    do {
                        m = rng.Next(5);
                    } while (m == n);
                }
                
                if (n == 0 || m == 0) {
                    yMissing = false;
                }
                if (n == 1 || m == 1) {
                    mMissing = false;
                }
                if (n == 2 || m == 2) {
                    dMissing = false;
                }
                if (n == 3 || m == 3) {
                    c1Missing = false;
                }
                if (n == 4 || m == 4) {
                    c2Missing = false;
                }
            }

            string y = year.ToString();
            string mo = month.ToString();
            string d = day.ToString();
            if (yMissing) {y = "";}
            if (mMissing) {mo = "";}
            if (dMissing) {d = "";}

            return BuildDateString(y, mo, d, c1Missing, c2Missing);
        } // MissingPart()

        private string DateOutOfBounds(int year, int month, int day, Random rng)
        {
            int minY = this.minDate.Year;
            int maxY = this.maxDate.Year;
            int minM = this.minDate.Month;
            int maxM = this.maxDate.Month;
            int minD = this.minDate.Day;
            int maxD = this.maxDate.Day;

            int y = year;
            int m = month;
            int d = day;

            int place = rng.Next(3);
            int dir = -1;

            if (place == 2) {
                if (minD > 1 && maxD < DateTime.DaysInMonth(maxY, maxM)) {
                    dir = rng.Next(2);
                }
                if (dir == 0 || (minD > 1 && maxD == DateTime.DaysInMonth(maxY, maxM))) {
                    d = rng.Next(1, minD - 1);
                    m = minM;
                    y = minY;
                } else if (dir == 1 || (minD == 1 && maxD < DateTime.DaysInMonth(maxY, maxM))) {
                    d = rng.Next(maxD + 1, DateTime.DaysInMonth(maxY, maxM));
                    m = maxM;
                    y = maxY;
                } else {
                    place = 1;
                }
            }

            if (place == 1) {
                if (minM > 1 && maxM < 12) {
                    dir = rng.Next(2);
                }
                if (dir == 0 || (minM > 1 && maxM == 12)) {
                    m = rng.Next(1, minM - 1);
                    y = minY;
                } else if (dir == 1 || (minM == 1 && maxM < 12)) {
                    m = rng.Next(maxM + 1, 12);
                    y = maxY;
                } else {
                    place = 0;
                }
            }

            if (place == 0) {
                if (minY > 1 && maxY < 9999) {
                    dir = rng.Next(2);
                }
                if (dir == 0 || (minY > 1 && maxY == 9999)) {
                    y = rng.Next(1, minY - 1);
                } else if (maxY < 9999) {
                    y = rng.Next(maxY + 1, 9999);
                } else {
                    y = rng.Next(10000, 20000);
                }
            }

            return BuildDateString(y.ToString(), m.ToString(), d.ToString(), true, true);
        } // DateOutOfBounds()

        private string BuildDateString(string year, string month, string day, bool delim1, bool delim2)
        {
            string delim1Str = "";
            string delim2Str = "";
            string date = "";

            if (this.dateFormat == 1)
            {
                if (delim1) {delim1Str = ".";}
                if (delim2) {delim2Str = ".";}
                date = day + delim1Str + month + delim2Str + year;
            }
            else if (this.dateFormat == 2)
            {
                if (delim1) {delim1Str = "/";}
                if (delim2) {delim2Str = "/";}
                date = month + delim1Str + day + delim2Str + year;
            }
            else // dateFormat == 0
            {
                if (delim1) {delim1Str = "-";}
                if (delim2) {delim2Str = "-";}
                date = year + delim1Str + month + delim2Str + day;
            }
            return date;
        }

	}
}