using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class DataClassRegistry
    {
        private class DefaultValue
        {
            public string minimumValue { get; set; }
            public string maximumValue { get; set; }
            
            public DefaultValue(string minValue, string maxValue)
            {
                this.minimumValue = minValue;
                this.maximumValue = maxValue;
            }
        }

        private Dictionary<DataClassType, DefaultValue> defaults = null;

        public DataClassRegistry()
        {
            defaults = new Dictionary<DataClassType, DefaultValue>();
        }

        public void SetDefaultMinMaxValues(DataClassType dataClassType, string minValue, string maxValue)
        {
            defaults[dataClassType] = new DefaultValue(minValue, maxValue);
        }

        public string GetDefaultMinValue(DataClassType dataClassType)
        {
            if(defaults.ContainsKey(dataClassType))
            {
                return defaults[dataClassType].minimumValue;
            }

            throw new Exception("No key " + dataClassType.ToString() + " in DataClassRegistry");
        }

        public string GetDefaultMaxValue(DataClassType dataClassType)
        {
            if (defaults.ContainsKey(dataClassType))
            {
                return defaults[dataClassType].maximumValue;
            }

            throw new Exception("No key " + dataClassType.ToString() + " in DataClassRegistry");
        }
    }
}
