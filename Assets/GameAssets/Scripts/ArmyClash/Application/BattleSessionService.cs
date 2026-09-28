using System;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Simulation;

namespace SimpleArmyClash.Application
{
    public sealed class BattleSessionService : IDisposable
    {
        private readonly IBattleSimulationFactory _factory;
        private IBattleSimulation _simulation;
        private UnitState[] _units = Array.Empty<UnitState>();

        public bool HasSession { get; private set; }
        public bool HasResult { get; private set; }
        public BattleResult Result { get; private set; }
        public IBattleSimulation Simulation => _simulation;
        public UnitState[] Units => _units;

        public BattleSessionService(IBattleSimulationFactory factory)
        {
            _factory = factory;
        }

        public void Start(UnitState[] units)
        {
            Stop();
            _units = units;
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
            _units = Array.Empty<UnitState>();
            HasSession = false;
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
