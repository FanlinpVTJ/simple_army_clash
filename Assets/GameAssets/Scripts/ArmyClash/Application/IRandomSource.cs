namespace SimpleArmyClash.Application
{
    public interface IRandomSource
    {
        int Seed { get; }
        int Next(int maximumExclusive);
    }
}
