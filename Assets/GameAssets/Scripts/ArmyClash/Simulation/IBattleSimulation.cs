using System;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public interface IBattleSimulation
    {
        event Action<int, int> OnUnitAttacked;
        event Action<int> OnUnitDied;
        event Action<BattleResult> OnCompleted;

        int UnitCount { get; }
        bool IsComplete { get; }
        float ElapsedTime { get; }

        UnitState GetUnit(int index);
        int GetAliveCount(int armyIndex);
        void Step(float deltaTime);
    }
}
