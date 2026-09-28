using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class NearestEnemyTargetSelectionStrategy : ITargetSelectionStrategy
    {
        public int SelectTarget(UnitState unit, UnitState[] units)
        {
            int nearestIdentifier = UnitState.NO_TARGET;
            float nearestSquaredDistance = float.PositiveInfinity;

            for (int i = 0; i < units.Length; i++)
            {
                UnitState candidate = units[i];

                if (!candidate.IsAlive || candidate.ArmyIndex == unit.ArmyIndex)
                {
                    continue;
                }

                Vector3 offset = candidate.Position - unit.Position;
                offset.y = 0f;
                float squaredDistance = offset.sqrMagnitude;

                if (squaredDistance < nearestSquaredDistance ||
                    squaredDistance == nearestSquaredDistance && candidate.Identifier < nearestIdentifier)
                {
                    nearestIdentifier = candidate.Identifier;
                    nearestSquaredDistance = squaredDistance;
                }
            }

            return nearestIdentifier;
        }
    }
}
