using System;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulation : IBattleSimulation
    {
        public event Action<int, int> OnUnitAttacked;
        public event Action<int> OnUnitDied;
        public event Action<BattleResult> OnCompleted;

        private readonly BattleSimulationState _state;
        private readonly IBattleSystem[] _systems;

        public int UnitCount => _state.Units.Length;
        public bool IsComplete => _state.IsComplete;
        public float ElapsedTime => _state.ElapsedTime;

        public BattleSimulation(UnitState[] units, ITargetSelectionStrategy targetSelectionStrategy,
            BattleSimulationSettings settings)
        {
            _state = new BattleSimulationState(units);
            _systems = new IBattleSystem[]
            {
                new TargetSelectionSystem(targetSelectionStrategy),
                new MovementSystem(settings),
                new AttackSystem(settings),
                new DeathCleanupSystem(),
                new VictorySystem()
            };
        }

        public UnitState GetUnit(int index)
        {
            return _state.Units[index];
        }

        public int GetAliveCount(int armyIndex)
        {
            return _state.GetAliveCount(armyIndex);
        }

        public void Step(float deltaTime)
        {
            if (_state.IsComplete || deltaTime <= 0f)
            {
                return;
            }

            _state.BeginStep(deltaTime);

            for (int i = 0; i < _systems.Length; i++)
            {
                _systems[i].Step(_state, deltaTime);
            }

            PublishChanges();
        }

        private void PublishChanges()
        {
            for (int i = 0; i < _state.AttackCount; i++)
            {
                OnUnitAttacked?.Invoke(_state.Attackers[i], _state.AttackTargets[i]);
            }

            for (int i = 0; i < _state.DeathCount; i++)
            {
                OnUnitDied?.Invoke(_state.Deaths[i]);
            }

            if (_state.IsComplete)
            {
                OnCompleted?.Invoke(_state.Result);
            }
        }
    }
}
