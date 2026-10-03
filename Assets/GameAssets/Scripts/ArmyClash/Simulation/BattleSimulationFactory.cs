using SimpleArmyClash.Ecs;
using Zenject;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulationFactory : IBattleSimulationFactory
    {
        private readonly IInstantiator _instantiator;

        public BattleSimulationFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public IBattleSimulation Create(UnitWorld units)
        {
            BattleSimulationState state = new BattleSimulationState(units);
            object[] arguments = { state };
            IBattleSystem[] systems =
            {
                _instantiator.Instantiate<TargetSelectionSystem>(arguments),
                _instantiator.Instantiate<MovementSystem>(arguments),
                _instantiator.Instantiate<AttackSystem>(arguments),
                _instantiator.Instantiate<DeathCleanupSystem>(arguments),
                _instantiator.Instantiate<VictorySystem>(arguments)
            };
            BattleSimulation simulation = new BattleSimulation(state, systems);
            return simulation;
        }
    }
}
