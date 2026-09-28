using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public interface ITargetSelectionStrategy
    {
        int SelectTarget(UnitState unit, UnitState[] units);
    }
}
