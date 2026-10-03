using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class NearestEnemyTargetSelectionStrategy : ITargetSelectionStrategy
    {
        public Entity SelectTarget(Entity unit, UnitWorld world, out bool hasTarget)
        {
            Entity nearest = default;
            int nearestIdentifier = int.MaxValue;
            float nearestSquaredDistance = float.PositiveInfinity;
            int armyIndex = world.Units.Get(unit).ArmyIndex;
            Vector3 position = world.Positions.Get(unit).Value;
            hasTarget = false;

            foreach (Entity candidate in world.LivingUnits)
            {
                ref UnitComponent other = ref world.Units.Get(candidate);

                if (world.Health.Get(candidate).Current <= 0 || other.ArmyIndex == armyIndex)
                {
                    continue;
                }

                Vector3 offset = world.Positions.Get(candidate).Value - position;
                offset.y = 0f;
                float squaredDistance = offset.sqrMagnitude;

                if (squaredDistance < nearestSquaredDistance
                    || squaredDistance == nearestSquaredDistance && other.Identifier < nearestIdentifier)
                {
                    nearest = candidate;
                    nearestIdentifier = other.Identifier;
                    nearestSquaredDistance = squaredDistance;
                    hasTarget = true;
                }
            }

            return nearest;
        }
    }
}
