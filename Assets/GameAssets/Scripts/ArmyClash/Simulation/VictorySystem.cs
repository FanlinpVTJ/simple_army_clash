namespace SimpleArmyClash.Simulation
{
    public sealed class VictorySystem : IBattleSystem
    {
        public void Step(BattleSimulationState state, float deltaTime)
        {
            if (state.GetAliveCount(0) == 0)
            {
                state.Complete(1);
            }
            else if (state.GetAliveCount(1) == 0)
            {
                state.Complete(0);
            }
        }
    }
}
