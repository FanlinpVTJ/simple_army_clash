using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;

namespace SimpleArmyClash.Simulation
{
    public interface ITargetSelectionStrategy
    {
        Entity SelectTarget(Entity unit, UnitWorld world, out bool hasTarget);
    }
}
