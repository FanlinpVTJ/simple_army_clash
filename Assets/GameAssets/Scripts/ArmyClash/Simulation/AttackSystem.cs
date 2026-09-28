using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class AttackSystem : IBattleSystem
    {
        private const float TIME_TOLERANCE = 0.0001f;

        private readonly BattleSimulationSettings _settings;

        public AttackSystem(BattleSimulationSettings settings)
        {
            _settings = settings;
        }

        public void Step(BattleSimulationState state, float deltaTime)
        {
            int readyCount = PrepareAttackOrder(state);

            for (int i = 0; i < readyCount; i++)
            {
                UnitState attacker = state.Units[state.AttackOrder[i]];

                if (!CanAttack(attacker, state))
                {
                    continue;
                }

                UnitState target = state.Units[attacker.TargetIdentifier];
                target.ReceiveDamage(attacker.Definition.Statistics.AttackDamage);
                attacker.ScheduleAttack(state.ElapsedTime + attacker.Definition.Statistics.AttackInterval);
                state.RecordAttack(attacker.Identifier, target.Identifier);
            }
        }

        private int PrepareAttackOrder(BattleSimulationState state)
        {
            int readyCount = 0;

            for (int i = 0; i < state.Units.Length; i++)
            {
                UnitState attacker = state.Units[i];

                if (!CanAttack(attacker, state))
                {
                    continue;
                }

                int insertIndex = readyCount;

                while (insertIndex > 0 && HasEarlierAttack(attacker, state.Units[state.AttackOrder[insertIndex - 1]]))
                {
                    state.AttackOrder[insertIndex] = state.AttackOrder[insertIndex - 1];
                    insertIndex--;
                }

                state.AttackOrder[insertIndex] = i;
                readyCount++;
            }

            return readyCount;
        }

        private bool CanAttack(UnitState attacker, BattleSimulationState state)
        {
            if (!attacker.IsAlive || attacker.TargetIdentifier == UnitState.NO_TARGET ||
                attacker.NextAttackTime > state.ElapsedTime + TIME_TOLERANCE)
            {
                return false;
            }

            UnitState target = state.Units[attacker.TargetIdentifier];

            if (!target.IsAlive)
            {
                return false;
            }

            Vector3 offset = target.Position - attacker.Position;
            offset.y = 0f;
            float attackDistance = attacker.Definition.Radius + target.Definition.Radius +
                _settings.MeleeReach + MovementSystem.CONTACT_TOLERANCE;
            return offset.sqrMagnitude <= attackDistance * attackDistance;
        }

        private bool HasEarlierAttack(UnitState attacker, UnitState other)
        {
            if (attacker.NextAttackTime < other.NextAttackTime)
            {
                return true;
            }

            if (attacker.NextAttackTime > other.NextAttackTime)
            {
                return false;
            }

            return attacker.AttackPriority < other.AttackPriority;
        }
    }
}
