using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public sealed class SpatialIndexSystem : BattleSystem
    {
        private readonly BattleSimulationSettings _settings;

        public SpatialIndexSystem(BattleSimulationState state, BattleSimulationSettings settings) : base(state)
        {
            _settings = settings;
        }

        public override void OnAwake()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            State.SpatialIndex.Rebuild(Units, _settings.MeleeReach);
        }
    }
}
