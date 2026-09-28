using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public interface IArmyGenerator
    {
        UnitDefinition[] Generate(int count);
    }
}
