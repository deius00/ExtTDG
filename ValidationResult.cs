using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class ValidationResult
    {
        public string generatorName { get; set; }
        public bool isValid { get; set; }

        public List<string> messages { get; set; }

        public ValidationResult()
        {
            this.generatorName = "";
            this.isValid = false;
            this.messages = new List<string>();
        }
    }
}
