namespace SimpleArmyClash.Simulation
{
    public sealed class DeathCleanupSystem : IBattleSystem
    {
        public void Step(BattleSimulationState state, float deltaTime)
        {
            state.RemoveDeadUnits();
        }
    }
}
