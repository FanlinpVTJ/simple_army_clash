using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public interface IUnitCatalog
    {
        int Count { get; }
        float MaximumRadius { get; }
        UnitDefinition GetDefinition(int index);
    }
}
