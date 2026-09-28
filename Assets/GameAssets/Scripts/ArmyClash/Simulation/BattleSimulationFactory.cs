using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulationFactory : IBattleSimulationFactory
    {
        private readonly ITargetSelectionStrategy _targetSelectionStrategy;
        private readonly BattleSimulationSettings _settings;

        public BattleSimulationFactory(ITargetSelectionStrategy targetSelectionStrategy,
            BattleSimulationSettings settings)
        {
            _targetSelectionStrategy = targetSelectionStrategy;
            _settings = settings;
        }

        public IBattleSimulation Create(UnitState[] units)
        {
            BattleSimulation simulation = new BattleSimulation(units, _targetSelectionStrategy, _settings);
            return simulation;
        }
    }
}
