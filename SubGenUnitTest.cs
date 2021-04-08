using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtTDG
{
    class SubGenUnitTest
    {
        public void Main()
        {
            Stopwatch sw = new Stopwatch();
            Random rng = new Random();
            List<string> results;

            sw.Start();

            // Create a generator
            GeneratorDate gen = new GeneratorDate("0", "abcde", "20000101", "20210407", true, false);

            // Generate data
            results = gen.Generate(10, 0.1, rng);

            sw.Stop();

            // Print to check output and time spent generating
            if (results.Count > 100)
                for (int i = 0; i < 100; i++)
                    Console.WriteLine(results[i]);
            else
                Console.WriteLine(results.ToString());
            Console.WriteLine("Time spent by generator was " + sw.ElapsedMilliseconds + " ms.");

        }
    }
}
