using System;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using SimpleArmyClash.Simulation;

namespace SimpleArmyClash.Application
{
    public sealed class BattleSessionService : IDisposable
    {
        private readonly IBattleSimulationFactory _factory;
        private IBattleSimulation _simulation;

        public bool HasSession { get; private set; }
        public bool HasResult { get; private set; }
        public BattleResult Result { get; private set; }
        public IBattleSimulation Simulation => _simulation;
        public UnitWorld Units => _simulation.Units;

        public BattleSessionService(IBattleSimulationFactory factory)
        {
            _factory = factory;
        }

        public void Start(UnitWorld units)
        {
            Stop();
            _simulation = _factory.Create(units);
            _simulation.OnCompleted += HandleCompleted;
            HasSession = true;
            HasResult = false;
        }

        public void Stop()
        {
            if (!HasSession)
            {
                return;
            }

            _simulation.OnCompleted -= HandleCompleted;
            _simulation.Dispose();
            HasSession = false;
            HasResult = false;
        }

        public void Dispose()
        {
            Stop();
        }

        private void HandleCompleted(BattleResult result)
        {
            Result = result;
            HasResult = true;
        }
    }
}
