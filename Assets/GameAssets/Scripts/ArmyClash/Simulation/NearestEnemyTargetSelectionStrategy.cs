using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class NearestEnemyTargetSelectionStrategy : ITargetSelectionStrategy
    {
        public Entity SelectTarget(Entity unit, UnitWorld world, IUnitSpatialIndex spatialIndex, out bool hasTarget)
        {
            int armyIndex = world.Units.Get(unit).ArmyIndex;
            Vector3 position = world.Positions.Get(unit).Value;
            return spatialIndex.FindNearestEnemy(position, armyIndex, out hasTarget);
        }
    }
}
