namespace SimpleArmyClash.Domain
{
    public readonly struct UnitStatistics
    {
        public int MaximumHealth { get; }
        public int AttackDamage { get; }
        public float MovementSpeed { get; }
        public float AttackInterval { get; }

        public UnitStatistics(int maximumHealth, int attackDamage, float movementSpeed, float attackInterval)
        {
            MaximumHealth = maximumHealth;
            AttackDamage = attackDamage;
            MovementSpeed = movementSpeed;
            AttackInterval = attackInterval;
        }
    }
}
