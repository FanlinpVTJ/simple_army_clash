using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public sealed class TargetSelectionSystem : IBattleSystem
    {
        private readonly ITargetSelectionStrategy _targetSelectionStrategy;

        public TargetSelectionSystem(ITargetSelectionStrategy targetSelectionStrategy)
        {
            _targetSelectionStrategy = targetSelectionStrategy;
        }

        public void Step(BattleSimulationState state, float deltaTime)
        {
            for (int i = 0; i < state.Units.Length; i++)
            {
                UnitState unit = state.Units[i];

                if (unit.IsAlive)
                {
                    unit.SetTarget(_targetSelectionStrategy.SelectTarget(unit, state.Units));
                }
            }
        }
    }
}
