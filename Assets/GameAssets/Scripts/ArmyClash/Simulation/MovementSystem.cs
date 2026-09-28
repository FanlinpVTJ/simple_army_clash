using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class MovementSystem : IBattleSystem
    {
        public const float CONTACT_TOLERANCE = 0.002f;

        private const float DIRECTION_TOLERANCE = 0.0001f;

        private readonly BattleSimulationSettings _settings;

        public MovementSystem(BattleSimulationSettings settings)
        {
            _settings = settings;
        }

        public void Step(BattleSimulationState state, float deltaTime)
        {
            for (int i = 0; i < state.Units.Length; i++)
            {
                state.NextPositions[i] = CalculatePosition(state.Units[i], state.Units, deltaTime);
            }

            for (int i = 0; i < state.Units.Length; i++)
            {
                state.Units[i].SetPosition(state.NextPositions[i]);
            }
        }

        private Vector3 CalculatePosition(UnitState unit, UnitState[] units, float deltaTime)
        {
            if (!unit.IsAlive || unit.TargetIdentifier == UnitState.NO_TARGET)
            {
                return unit.Position;
            }

            UnitState target = units[unit.TargetIdentifier];
            Vector3 offset = target.Position - unit.Position;
            offset.y = 0f;
            float distance = offset.magnitude;
            float contactDistance = unit.Definition.Radius + target.Definition.Radius + _settings.MeleeReach;

            if (distance <= contactDistance + CONTACT_TOLERANCE)
            {
                return unit.Position;
            }

            Vector3 direction = offset / distance;
            Vector3 tangent = new Vector3(-direction.z, 0f, direction.x);
            float separation = CalculateSeparation(unit, units, tangent);
            Vector3 movementDirection = (direction + tangent * separation * _settings.SeparationStrength).normalized;
            float remainingDistance = distance - contactDistance;
            float maximumMovement = unit.Definition.Statistics.MovementSpeed * deltaTime;

            if (target.TargetIdentifier == unit.Identifier)
            {
                float combinedSpeed = unit.Definition.Statistics.MovementSpeed +
                    target.Definition.Statistics.MovementSpeed;
                remainingDistance *= unit.Definition.Statistics.MovementSpeed / combinedSpeed;
            }

            float movementDistance = Mathf.Min(maximumMovement, remainingDistance);
            Vector3 position = unit.Position + movementDirection * movementDistance;
            return position;
        }

        private float CalculateSeparation(UnitState unit, UnitState[] units, Vector3 tangent)
        {
            float separation = 0f;

            for (int i = 0; i < units.Length; i++)
            {
                UnitState neighbor = units[i];

                if (!neighbor.IsAlive || neighbor.Identifier == unit.Identifier ||
                    neighbor.Identifier == unit.TargetIdentifier)
                {
                    continue;
                }

                Vector3 offset = unit.Position - neighbor.Position;
                offset.y = 0f;
                float minimumDistance = unit.Definition.Radius + neighbor.Definition.Radius + _settings.MeleeReach;
                float squaredDistance = offset.sqrMagnitude;

                if (squaredDistance >= minimumDistance * minimumDistance)
                {
                    continue;
                }

                float lateralDistance = Vector3.Dot(offset, tangent);
                float side = lateralDistance >= 0f ? 1f : -1f;

                if (Mathf.Abs(lateralDistance) < DIRECTION_TOLERANCE)
                {
                    side = unit.AttackPriority < neighbor.AttackPriority ? -1f : 1f;
                }

                separation += side * (1f - Mathf.Sqrt(squaredDistance) / minimumDistance);
            }

            return Mathf.Clamp(separation, -1f, 1f);
        }
    }
}
