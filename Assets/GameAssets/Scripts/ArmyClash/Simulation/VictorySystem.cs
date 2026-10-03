namespace SimpleArmyClash.Simulation
{
    public sealed class VictorySystem : BattleSystem
    {
        public VictorySystem(BattleSimulationState state) : base(state)
        {
        }

        public override void OnAwake()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            State.CountArmies();

            if (State.GetAliveCount(0) == 0)
            {
                State.Complete(1);
            }
            else if (State.GetAliveCount(1) == 0)
            {
                State.Complete(0);
            }
        }
    }
}
