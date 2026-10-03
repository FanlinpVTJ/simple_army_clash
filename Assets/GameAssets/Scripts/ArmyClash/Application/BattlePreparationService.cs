using System;
using Scellecs.Morpeh;
using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Application
{
    public sealed class BattlePreparationService : IDisposable
    {
        private readonly BattleConfiguration _configuration;
        private readonly IArmyGenerator _generator;
        private readonly IFormationLayout _formation;
        private readonly IRandomSource _random;
        private UnitWorld _units;
        private bool _hasWorld;

        public UnitWorld Units => _units;

        public BattlePreparationService(BattleConfiguration configuration, IArmyGenerator generator,
            IFormationLayout formation, IRandomSource random)
        {
            _configuration = configuration;
            _generator = generator;
            _formation = formation;
            _random = random;
        }

        public void Generate()
        {
            Dispose();
            _units = new UnitWorld(_configuration.ArmySize * 2);
            _hasWorld = true;
            SystemsGroup group = _units.World.CreateSystemsGroup();
            group.AddSystem(new FormationSwapSystem());
            _units.World.AddSystemsGroup(0, group);

            for (int armyIndex = 0; armyIndex < 2; armyIndex++)
            {
                UnitDefinition[] army = _generator.Generate(_configuration.ArmySize);

                for (int slotIndex = 0; slotIndex < army.Length; slotIndex++)
                {
                    int priority = armyIndex * _configuration.ArmySize + slotIndex;
                    Vector3 position = _formation.GetPosition(armyIndex, slotIndex);
                    _units.Spawn(armyIndex, army[slotIndex], position, priority);
                }
            }

            _units.World.Commit();
        }

        public bool TrySwap(int firstIdentifier, int secondIdentifier)
        {
            if (!_hasWorld || !_units.Contains(firstIdentifier) || !_units.Contains(secondIdentifier))
            {
                return false;
            }

            Entity first = _units.GetEntity(firstIdentifier);
            Entity second = _units.GetEntity(secondIdentifier);

            if (_units.Units.Get(first).ArmyIndex != _units.Units.Get(second).ArmyIndex)
            {
                return false;
            }

            ref FormationSwapRequest request = ref _units.World.GetStash<FormationSwapRequest>().Add(first);
            request.Other = second;
            _units.World.Update(0f);
            return true;
        }

        public UnitWorld CreateBattleUnits()
        {
            int[] priorities = new int[_units.UnitCount];

            for (int i = 0; i < priorities.Length; i++)
            {
                priorities[i] = i;
            }

            for (int i = priorities.Length - 1; i > 0; i--)
            {
                int selectedIndex = _random.Next(i + 1);
                int previousPriority = priorities[i];
                priorities[i] = priorities[selectedIndex];
                priorities[selectedIndex] = previousPriority;
            }

            UnitWorld result = new UnitWorld(_units.UnitCount);

            for (int i = 0; i < _units.UnitCount; i++)
            {
                Entity entity = _units.GetEntity(i);
                ref UnitComponent unit = ref _units.Units.Get(entity);
                result.Spawn(unit.ArmyIndex, unit.Definition, _units.Positions.Get(entity).Value, priorities[i]);
            }

            result.World.Commit();
            return result;
        }

        public void Dispose()
        {
            if (!_hasWorld)
            {
                return;
            }

            _units.Dispose();
            _hasWorld = false;
        }
    }
}
