using System;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public interface IBattleSimulation : IDisposable
    {
        event Action<int, Vector3> OnUnitAttacked;
        event Action<int> OnUnitDied;
        event Action<BattleResult> OnCompleted;

        UnitWorld Units { get; }
        int UnitCount { get; }
        bool IsComplete { get; }
        float ElapsedTime { get; }

        int GetAliveCount(int armyIndex);
        void Step(float deltaTime);
    }
}
