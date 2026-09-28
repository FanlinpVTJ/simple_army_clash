namespace SimpleArmyClash.Domain
{
    public sealed class BattleSimulationSettings
    {
        public float MeleeReach { get; }
        public float SeparationStrength { get; }

        public BattleSimulationSettings(float meleeReach = 0.1f, float separationStrength = 0.6f)
        {
            MeleeReach = meleeReach;
            SeparationStrength = separationStrength;
        }
    }
}
