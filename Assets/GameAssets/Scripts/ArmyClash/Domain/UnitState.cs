using UnityEngine;

namespace SimpleArmyClash.Domain
{
    public sealed class UnitState
    {
        public const int NO_TARGET = -1;

        public int Identifier { get; }
        public int ArmyIndex { get; }
        public UnitDefinition Definition { get; }
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;
        public Vector3 Position { get; private set; }
        public int TargetIdentifier { get; private set; }
        public float NextAttackTime { get; private set; }
        public int AttackPriority { get; }

        public UnitState(int identifier, int armyIndex, UnitDefinition definition, Vector3 position,
            int attackPriority)
        {
            Identifier = identifier;
            ArmyIndex = armyIndex;
            Definition = definition;
            Health = definition.Statistics.MaximumHealth;
            Position = position;
            TargetIdentifier = NO_TARGET;
            AttackPriority = attackPriority;
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public void SetTarget(int identifier)
        {
            TargetIdentifier = identifier;
        }

        public void ReceiveDamage(int damage)
        {
            Health = Mathf.Max(0, Health - damage);
        }

        public void ScheduleAttack(float time)
        {
            NextAttackTime = time;
        }
    }
}
