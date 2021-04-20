using System;
using System.Collections.Generic;

namespace ExtTDG
{
    public class DataClassRegistry
    {
        private class DefaultValue
        {
            public string allowedCharacters { get; set; }
            public string anomalyCharacters { get; set; }
            public string minimumValue { get; set; }
            public string maximumValue { get; set; }

            public bool hasAnomalies { get; set; }
            public bool isUnique { get; set; }
            
            public DefaultValue()
            {
                allowedCharacters = "";
                anomalyCharacters = "";
                minimumValue = "";
                maximumValue = "";
                hasAnomalies = false;
                isUnique = false;
            }
        }

        private Dictionary<DataClassType, DefaultValue> defaults = null;

        public DataClassRegistry()
        {
            defaults = new Dictionary<DataClassType, DefaultValue>();
        }

        public string GetDefaultAllowedChars(DataClassType type)
        {
            DefaultValue values = GetEntry(type);
            return values.allowedCharacters;
        }

        public string GetDefaultAnomalyChars(DataClassType type)
        {
            DefaultValue values = GetEntry(type);
            return values.anomalyCharacters;
        }

        public string GetDefaultMinValue(DataClassType type)
        {
            if(defaults.ContainsKey(type))
            {
                return defaults[type].minimumValue;
            }

            throw new Exception("No key " + type.ToString() + " in DataClassRegistry");
        }

        public string GetDefaultMaxValue(DataClassType type)
        {
            if (defaults.ContainsKey(type))
            {
                return defaults[type].maximumValue;
            }

            throw new Exception("No key " + type.ToString() + " in DataClassRegistry");
        }

        public bool GetDefaultHasAnomalies(DataClassType type)
        {

            if (defaults.ContainsKey(type))
            {
                return defaults[type].hasAnomalies;
            }

            throw new Exception("No key " + type.ToString() + " in DataClassRegistry");
        }

        public bool GetDefaultIsUnique(DataClassType type)
        {

            if (defaults.ContainsKey(type))
            {
                return defaults[type].isUnique;
            }

            throw new Exception("No key " + type.ToString() + " in DataClassRegistry");
        }

        public void SetDefaultMinMaxValues(DataClassType type, string minValue, string maxValue)
        {
            DefaultValue values = GetEntry(type);
            values.minimumValue = minValue;
            values.maximumValue = maxValue;
            defaults[type] = values;
        }

        public void SetDefaultCharacters(DataClassType type, string allowedChars, string anomalyChars)
        {
            DefaultValue values = GetEntry(type);
            values.allowedCharacters = allowedChars;
            values.anomalyCharacters = anomalyChars;
            defaults[type] = values;
        }

        public void SetDefaultHasAnomalies(DataClassType type, bool hasAnomalies)
        {
            DefaultValue values = GetEntry(type);
            values.hasAnomalies = hasAnomalies;
            defaults[type] = values;
        }

        public void SetDefaultIsUnique(DataClassType type, bool isUnique)
        {
            DefaultValue values = GetEntry(type);
            values.isUnique = isUnique;
            defaults[type] = values;
        }

        // Get existing data or create new DefaultValue-instance
        private DefaultValue GetEntry(DataClassType type)
        {
            if (defaults.ContainsKey(type))
            {
                return defaults[type];
            }
            else
            {
                DefaultValue newValues = new DefaultValue();
                return newValues;
            }
        }
    }
}
