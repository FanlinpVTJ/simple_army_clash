using System;
using Scellecs.Morpeh;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulation : IBattleSimulation
    {
        public event Action<int, Vector3> OnUnitAttacked;
        public event Action<int> OnUnitDied;
        public event Action<BattleResult> OnCompleted;

        private readonly BattleSimulationState _state;
        private bool _disposed;

        public UnitWorld Units => _state.Units;
        public int UnitCount => Units.UnitCount;
        public bool IsComplete => _state.IsComplete;
        public float ElapsedTime => _state.ElapsedTime;

        public BattleSimulation(BattleSimulationState state, IBattleSystem[] systems)
        {
            _state = state;
            SystemsGroup group = Units.World.CreateSystemsGroup();

            for (int i = 0; i < systems.Length; i++)
            {
                group.AddSystem(systems[i]);
            }

            Units.World.AddSystemsGroup(0, group);
        }

        public int GetAliveCount(int armyIndex)
        {
            return _state.GetAliveCount(armyIndex);
        }

        public void Step(float deltaTime)
        {
            if (_disposed || _state.IsComplete || deltaTime <= 0f)
            {
                return;
            }

            _state.BeginStep(deltaTime);
            Units.World.Update(deltaTime);
            PublishChanges();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            OnUnitAttacked = null;
            OnUnitDied = null;
            OnCompleted = null;
            Units.Dispose();
        }

        private void PublishChanges()
        {
            for (int i = 0; i < _state.AttackCount; i++)
            {
                OnUnitAttacked?.Invoke(_state.Attackers[i], _state.AttackPositions[i]);
            }

            for (int i = 0; i < _state.DeathCount; i++)
            {
                Entity entity = _state.Deaths[i];
                OnUnitDied?.Invoke(Units.Units.Get(entity).Identifier);
                Units.World.RemoveEntity(entity);
            }

            Units.World.Commit();

            if (_state.IsComplete)
            {
                OnCompleted?.Invoke(_state.Result);
            }
        }
    }
}
