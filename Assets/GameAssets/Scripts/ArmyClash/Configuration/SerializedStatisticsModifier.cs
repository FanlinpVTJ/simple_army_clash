using System;
using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [Serializable]
    public sealed class SerializedStatisticsModifier
    {
        [SerializeField]
        private int _health;

        [SerializeField]
        private int _attackDamage;

        [SerializeField]
        private float _movementSpeed;

        [SerializeField, Tooltip("Seconds added to the interval between attacks.")]
        private float _attackInterval;

        public int Health => _health;
        public int AttackDamage => _attackDamage;
        public float MovementSpeed => _movementSpeed;
        public float AttackInterval => _attackInterval;

        public SerializedStatisticsModifier()
        {
        }

        public SerializedStatisticsModifier(int health, int attackDamage, float movementSpeed, float attackInterval)
        {
            _health = health;
            _attackDamage = attackDamage;
            _movementSpeed = movementSpeed;
            _attackInterval = attackInterval;
        }
    }
}
