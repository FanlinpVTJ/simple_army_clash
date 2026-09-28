using System;

namespace SimpleArmyClash.Application
{
    public sealed class SeededRandomSource : IRandomSource
    {
        private readonly Random _random;

        public int Seed { get; }

        public SeededRandomSource(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        public int Next(int maximumExclusive)
        {
            return _random.Next(maximumExclusive);
        }
    }
}
