using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulationState
    {
        private readonly int[] _aliveCounts = new int[2];
        private readonly bool[] _activeUnits;

        public UnitState[] Units { get; }
        public Vector3[] NextPositions { get; }
        public int[] AttackOrder { get; }
        public int[] Attackers { get; }
        public int[] AttackTargets { get; }
        public int[] Deaths { get; }
        public int AttackCount { get; private set; }
        public int DeathCount { get; private set; }
        public float ElapsedTime { get; private set; }
        public bool IsComplete { get; private set; }
        public BattleResult Result { get; private set; }

        public BattleSimulationState(UnitState[] units)
        {
            Units = units;
            NextPositions = new Vector3[units.Length];
            AttackOrder = new int[units.Length];
            Attackers = new int[units.Length];
            AttackTargets = new int[units.Length];
            Deaths = new int[units.Length];
            _activeUnits = new bool[units.Length];

            for (int i = 0; i < units.Length; i++)
            {
                UnitState unit = units[i];
                _activeUnits[i] = unit.IsAlive;

                if (unit.IsAlive)
                {
                    _aliveCounts[unit.ArmyIndex]++;
                }
            }
        }

        public void BeginStep(float deltaTime)
        {
            ElapsedTime += deltaTime;
            AttackCount = 0;
            DeathCount = 0;
        }

        public int GetAliveCount(int armyIndex)
        {
            return _aliveCounts[armyIndex];
        }

        public void RecordAttack(int attackerIdentifier, int targetIdentifier)
        {
            Attackers[AttackCount] = attackerIdentifier;
            AttackTargets[AttackCount] = targetIdentifier;
            AttackCount++;
        }

        public void RemoveDeadUnits()
        {
            for (int i = 0; i < Units.Length; i++)
            {
                UnitState unit = Units[i];

                if (!_activeUnits[i] || unit.IsAlive)
                {
                    continue;
                }

                _activeUnits[i] = false;
                _aliveCounts[unit.ArmyIndex]--;
                Deaths[DeathCount] = unit.Identifier;
                DeathCount++;
                unit.SetTarget(UnitState.NO_TARGET);
            }
        }

        public void Complete(int winnerArmyIndex)
        {
            IsComplete = true;
            Result = new BattleResult(winnerArmyIndex, ElapsedTime, _aliveCounts[winnerArmyIndex]);
        }
    }
}
