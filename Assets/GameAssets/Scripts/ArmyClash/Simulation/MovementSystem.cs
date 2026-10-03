using System.Collections.Generic;
using Scellecs.Morpeh;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class MovementSystem : BattleSystem
    {
        public const float CONTACT_TOLERANCE = 0.002f;

        private const float DIRECTION_TOLERANCE = 0.0001f;
        private readonly BattleSimulationSettings _settings;
        private readonly List<Entity> _neighbors;
        private Filter _filter;

        public MovementSystem(BattleSimulationState state, BattleSimulationSettings settings) : base(state)
        {
            _settings = settings;
            _neighbors = new List<Entity>(state.Units.UnitCount);
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<AliveComponent>().With<PositionComponent>().With<MovementComponent>()
                .With<TargetComponent>().With<HealthComponent>().With<AttackComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_neighbors.Capacity < Units.UnitCount)
            {
                _neighbors.Capacity = Units.UnitCount;
            }

            foreach (Entity entity in _filter)
            {
                Units.Movement.Get(entity).NextPosition = CalculatePosition(entity, deltaTime);
            }

            foreach (Entity entity in _filter)
            {
                Units.Positions.Get(entity).Value = Units.Movement.Get(entity).NextPosition;
            }
        }

        private Vector3 CalculatePosition(Entity entity, float deltaTime)
        {
            ref PositionComponent position = ref Units.Positions.Get(entity);
            ref TargetComponent target = ref Units.Targets.Get(entity);

            if (Units.Health.Get(entity).Current <= 0 || !target.HasTarget || !Units.IsAlive(target.Entity))
            {
                return position.Value;
            }

            ref PositionComponent targetPosition = ref Units.Positions.Get(target.Entity);
            Vector3 offset = targetPosition.Value - position.Value;
            offset.y = 0f;
            float distance = offset.magnitude;
            float contactDistance = position.Radius + targetPosition.Radius + _settings.MeleeReach;

            if (distance <= contactDistance + CONTACT_TOLERANCE)
            {
                return position.Value;
            }

            Vector3 direction = offset / distance;
            Vector3 tangent = new Vector3(-direction.z, 0f, direction.x);
            float separation = CalculateSeparation(entity, target.Entity, tangent);
            Vector3 movementDirection = (direction + tangent * separation * _settings.SeparationStrength).normalized;
            float remainingDistance = distance - contactDistance;
            float speed = Units.Movement.Get(entity).Speed;
            ref TargetComponent opponentTarget = ref Units.Targets.Get(target.Entity);

            if (opponentTarget.HasTarget && opponentTarget.Entity == entity)
            {
                float combinedSpeed = speed + Units.Movement.Get(target.Entity).Speed;
                remainingDistance *= speed / combinedSpeed;
            }

            float movementDistance = Mathf.Min(speed * deltaTime, remainingDistance);
            return position.Value + movementDirection * movementDistance;
        }

        private float CalculateSeparation(Entity entity, Entity target, Vector3 tangent)
        {
            float separation = 0f;
            ref PositionComponent position = ref Units.Positions.Get(entity);

            float searchRadius = position.Radius + State.SpatialIndex.MaximumRadius + _settings.MeleeReach;
            State.SpatialIndex.CollectNeighbors(position.Value, searchRadius, _neighbors);

            for (int i = 0; i < _neighbors.Count; i++)
            {
                Entity neighbor = _neighbors[i];

                if (neighbor == entity || neighbor == target || Units.Health.Get(neighbor).Current <= 0)
                {
                    continue;
                }

                ref PositionComponent neighborPosition = ref Units.Positions.Get(neighbor);
                Vector3 offset = position.Value - neighborPosition.Value;
                offset.y = 0f;
                float minimumDistance = position.Radius + neighborPosition.Radius + _settings.MeleeReach;
                float squaredDistance = offset.sqrMagnitude;

                if (squaredDistance >= minimumDistance * minimumDistance)
                {
                    continue;
                }

                float lateralDistance = Vector3.Dot(offset, tangent);
                float side = lateralDistance >= 0f ? 1f : -1f;

                if (Mathf.Abs(lateralDistance) < DIRECTION_TOLERANCE)
                {
                    side = Units.Attacks.Get(entity).Priority < Units.Attacks.Get(neighbor).Priority ? -1f : 1f;
                }

                separation += side * (1f - Mathf.Sqrt(squaredDistance) / minimumDistance);
            }

            return Mathf.Clamp(separation, -1f, 1f);
        }
    }
}
