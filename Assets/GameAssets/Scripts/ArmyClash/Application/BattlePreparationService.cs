using System;
using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Application
{
    public sealed class BattlePreparationService
    {
        private readonly BattleConfiguration _configuration;
        private readonly IArmyGenerator _generator;
        private readonly IFormationLayout _formation;
        private readonly IRandomSource _random;
        private UnitState[] _units = Array.Empty<UnitState>();

        public UnitState[] Units => _units;

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
            _units = new UnitState[_configuration.ArmySize * 2];

            for (int armyIndex = 0; armyIndex < 2; armyIndex++)
            {
                UnitDefinition[] army = _generator.Generate(_configuration.ArmySize);

                for (int slotIndex = 0; slotIndex < army.Length; slotIndex++)
                {
                    int identifier = armyIndex * _configuration.ArmySize + slotIndex;
                    Vector3 position = _formation.GetPosition(armyIndex, slotIndex);
                    _units[identifier] = new UnitState(identifier, armyIndex, army[slotIndex], position, identifier);
                }
            }
        }

        public bool TrySwap(int firstIdentifier, int secondIdentifier)
        {
            if (firstIdentifier < 0 || firstIdentifier >= _units.Length
                || secondIdentifier < 0 || secondIdentifier >= _units.Length)
            {
                return false;
            }

            UnitState first = _units[firstIdentifier];
            UnitState second = _units[secondIdentifier];

            if (first.ArmyIndex != second.ArmyIndex)
            {
                return false;
            }

            Vector3 firstPosition = first.Position;
            first.SetPosition(second.Position);
            second.SetPosition(firstPosition);
            return true;
        }

        public UnitState[] CreateBattleUnits()
        {
            int[] priorities = new int[_units.Length];

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

            UnitState[] result = new UnitState[_units.Length];

            for (int i = 0; i < _units.Length; i++)
            {
                UnitState unit = _units[i];
                result[i] = new UnitState(unit.Identifier, unit.ArmyIndex, unit.Definition, unit.Position, priorities[i]);
            }

            return result;
        }
    }
}
