using System;
using System.Collections.Generic;


/// <summary>
/// Interface for generator.
/// Generator must provide now only Generate()-method.
/// </summary>
namespace ExtTDG
{
    interface IGenerator
    {
        List<string> Generate(int numItems, double anomalyChance, Random rng);
    }
}
