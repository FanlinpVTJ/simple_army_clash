using UnityEngine;

namespace SimpleArmyClash.Domain
{
    public readonly struct UnitState
    {
        public int Identifier { get; }
        public int ArmyIndex { get; }
        public UnitDefinition Definition { get; }
        public Vector3 Position { get; }
        public int Health { get; }

        public UnitState(int identifier, int armyIndex, UnitDefinition definition, Vector3 position, int health)
        {
            Identifier = identifier;
            ArmyIndex = armyIndex;
            Definition = definition;
            Position = position;
            Health = health;
        }
    }
}
