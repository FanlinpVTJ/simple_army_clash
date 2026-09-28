namespace SimpleArmyClash.Simulation
{
    public interface IBattleSystem
    {
        void Step(BattleSimulationState state, float deltaTime);
    }
}
