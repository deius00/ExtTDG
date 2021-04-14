using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class GeneratorParameters
    {
        public DataClassType dataClassType  { get; set; }
        public string dataClassTypeName     { get; set; }
        public string allowedCharacters     { get; set; }
        public string anomalyCharacters     { get; set; }
        public string minLength             { get; set; }
        public string maxLength             { get; set; }
        public bool hasAnomalies            { get; set; }
        public bool isUnique                { get; set; }

        public GeneratorParameters()
        {
            this.dataClassTypeName = null;
            this.allowedCharacters = null;
            this.anomalyCharacters = null;
            this.minLength = null;
            this.maxLength = null;
            this.hasAnomalies = false;
            this.isUnique = false;
        }
    }
}
