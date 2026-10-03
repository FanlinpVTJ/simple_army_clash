using Scellecs.Morpeh;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class AttackSystem : BattleSystem
    {
        private const float TIME_TOLERANCE = 0.0001f;
        private readonly BattleSimulationSettings _settings;
        private Filter _filter;

        public AttackSystem(BattleSimulationState state, BattleSimulationSettings settings) : base(state)
        {
            _settings = settings;
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<AliveComponent>().With<UnitComponent>().With<AttackComponent>()
                .With<TargetComponent>().With<HealthComponent>().With<PositionComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            int readyCount = PrepareAttackOrder();

            for (int i = 0; i < readyCount; i++)
            {
                Entity attacker = State.AttackOrder[i];

                if (!CanAttack(attacker))
                {
                    continue;
                }

                Entity target = Units.Targets.Get(attacker).Entity;
                ref AttackComponent attack = ref Units.Attacks.Get(attacker);
                ref HealthComponent health = ref Units.Health.Get(target);
                health.Current = Mathf.Max(0, health.Current - attack.Damage);
                attack.ReadyAt = State.ElapsedTime + attack.Interval;
                State.RecordAttack(attacker, target);
            }
        }

        private int PrepareAttackOrder()
        {
            int readyCount = 0;

            foreach (Entity attacker in _filter)
            {
                if (!CanAttack(attacker))
                {
                    continue;
                }

                int insertIndex = readyCount;

                while (insertIndex > 0 && HasEarlierAttack(attacker, State.AttackOrder[insertIndex - 1]))
                {
                    State.AttackOrder[insertIndex] = State.AttackOrder[insertIndex - 1];
                    insertIndex--;
                }

                State.AttackOrder[insertIndex] = attacker;
                readyCount++;
            }

            return readyCount;
        }

        private bool CanAttack(Entity attacker)
        {
            ref TargetComponent target = ref Units.Targets.Get(attacker);

            if (!Units.IsAlive(attacker) || !target.HasTarget || !Units.IsAlive(target.Entity)
                || Units.Attacks.Get(attacker).ReadyAt > State.ElapsedTime + TIME_TOLERANCE)
            {
                return false;
            }

            ref PositionComponent position = ref Units.Positions.Get(attacker);
            ref PositionComponent targetPosition = ref Units.Positions.Get(target.Entity);
            Vector3 offset = targetPosition.Value - position.Value;
            offset.y = 0f;
            float attackDistance = position.Radius + targetPosition.Radius
                + _settings.MeleeReach + MovementSystem.CONTACT_TOLERANCE;
            return offset.sqrMagnitude <= attackDistance * attackDistance;
        }

        private bool HasEarlierAttack(Entity attacker, Entity other)
        {
            ref AttackComponent attack = ref Units.Attacks.Get(attacker);
            ref AttackComponent otherAttack = ref Units.Attacks.Get(other);

            if (attack.ReadyAt < otherAttack.ReadyAt)
            {
                return true;
            }

            if (attack.ReadyAt > otherAttack.ReadyAt)
            {
                return false;
            }

            return attack.Priority < otherAttack.Priority;
        }
    }
}
