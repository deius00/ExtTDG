using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG.Data
{
    public class ErrorText
    {
        // Allowed / anomaly characters error texts
        public static readonly string kErrAllowedCharsEmpty = "Allowed characters empty";
        public static readonly string kErrAnomalyCharsEmpty = "Anomaly characters empty";
        public static readonly string kErrSharedCharacters = "Same characters in allowed and anomaly characters";

        // Minimum length / value error texts
        public static readonly string kErrParseMinLen = "Cannot parse minimum length";
        public static readonly string kErrParseMinVal = "Cannot parse minimum value";
        public static readonly string kErrMinGEMaxLen = "Minimum length is greater than or equal to maximum length";
        public static readonly string kErrMinGEMaxVal = "Minimum value is greater than or equal to maximum value";
        public static readonly string kErrMinLZeroLen = "Minimum length less than zero";
        public static readonly string kErrMinLZeroVal = "Minimum value less than zero";
        public static readonly string kErrMinNoZeroAllowedLen = "Minimum length cannot be zero";
        public static readonly string kErrMinNoZeroAllowedVal = "Minimum value cannot be zero";
        public static readonly string kErrMinNoLessThanLen = "Minimum length cannot be less than ";
        public static readonly string kErrMinNoLessThanVal = "Minimum value cannot be less than ";

        // Maximum length / value error texts
        public static readonly string kErrParseMaxLen = "Cannot parse maximum length";
        public static readonly string kErrParseMaxVal = "Cannot parse maximum value";
        public static readonly string kErrMaxLEMinLen = "Maximum length less than minimum length";
        public static readonly string kErrMaxLEMinVal = "Maximum value less than minimum value";
        public static readonly string kErrMaxNoZeroAllowedLen = "Maximum length cannot be zero";
        public static readonly string kErrMaxNoZeroAllowedVal = "Maximum value cannot be zero";
        public static readonly string kErrMaxNoLessThanLen = "Maximum length cannot be less than ";
        public static readonly string kErrMaxNoLessThanVal = "Maximum value cannot be less than ";

        // Unique
        public static readonly string kErrNoUniqueGuaranteeExpandRange = "Cannot guarantee uniqueness, expand min/max range";
        public static readonly string kErrNoUniqueGuaranteeRaiseToMaxLen = "Cannot guarantee uniqueness, raise maximum length to ";
        public static readonly string kErrNoUniqueGuaranteeRaiseToMaxVal = "Cannot guarantee uniqueness, raise maximum value to ";

        // Item count
        public static readonly string kErrTooManyItems = "Too many items, expand min/max range";
    }
}
