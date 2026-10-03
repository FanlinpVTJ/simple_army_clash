using System;
using Scellecs.Morpeh;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public sealed class BattleSimulationState
    {
        private readonly int[] _aliveCounts = new int[2];
        private Entity[] _attackOrder;
        private int[] _attackers;
        private Vector3[] _attackPositions;
        private Entity[] _deaths;

        public UnitWorld Units { get; }
        public Entity[] AttackOrder => _attackOrder;
        public int[] Attackers => _attackers;
        public Vector3[] AttackPositions => _attackPositions;
        public Entity[] Deaths => _deaths;
        public int AttackCount { get; private set; }
        public int DeathCount { get; private set; }
        public float ElapsedTime { get; private set; }
        public bool IsComplete { get; private set; }
        public BattleResult Result { get; private set; }

        public BattleSimulationState(UnitWorld units)
        {
            Units = units;
            _attackOrder = new Entity[units.UnitCount];
            _attackers = new int[units.UnitCount];
            _attackPositions = new Vector3[units.UnitCount];
            _deaths = new Entity[units.UnitCount];
            CountArmies();
        }

        public void BeginStep(float deltaTime)
        {
            if (_attackOrder.Length < Units.UnitCount)
            {
                Array.Resize(ref _attackOrder, Units.UnitCount);
                Array.Resize(ref _attackers, Units.UnitCount);
                Array.Resize(ref _attackPositions, Units.UnitCount);
                Array.Resize(ref _deaths, Units.UnitCount);
            }

            ElapsedTime += deltaTime;
            AttackCount = 0;
            DeathCount = 0;
        }

        public int GetAliveCount(int armyIndex)
        {
            return _aliveCounts[armyIndex];
        }

        public void RecordAttack(Entity attacker, Entity target)
        {
            _attackers[AttackCount] = Units.Units.Get(attacker).Identifier;
            _attackPositions[AttackCount] = Units.Positions.Get(target).Value;
            AttackCount++;
        }

        public void RecordDeath(Entity entity)
        {
            _deaths[DeathCount] = entity;
            DeathCount++;
        }

        public void CountArmies()
        {
            Array.Clear(_aliveCounts, 0, _aliveCounts.Length);

            foreach (Entity entity in Units.LivingUnits)
            {
                if (Units.Health.Get(entity).Current > 0)
                {
                    _aliveCounts[Units.Units.Get(entity).ArmyIndex]++;
                }
            }
        }

        public void Complete(int winnerArmyIndex)
        {
            IsComplete = true;
            Result = new BattleResult(winnerArmyIndex, ElapsedTime, _aliveCounts[winnerArmyIndex]);
        }
    }
}
