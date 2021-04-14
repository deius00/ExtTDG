using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    public class GeneratorStats
    {
        public DataClassType type { get; set; }
        public long durationInMilliseconds { get; set; }

        public GeneratorStats()
        {
            durationInMilliseconds = 0;
        }

        public GeneratorStats(DataClassType type, long durationInMilliseconds)
        {
            this.type = type;
            this.durationInMilliseconds = durationInMilliseconds;
        }
    }
}
