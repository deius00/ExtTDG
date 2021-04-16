using System;
using System.Collections.Generic;


/// <summary>
/// Interface for generator.
/// </summary>
namespace ExtTDG
{
    interface IGenerator
    {
        bool Validate(int numItems, out ValidationResult result);
        List<string> Generate(int numItems, double anomalyChance, Random rng);
    }
}
