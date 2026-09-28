using System;
using SimpleArmyClash.Configuration;
using UnityEngine;

namespace SimpleArmyClash.Application
{
    public sealed class GridFormationLayout : IFormationLayout
    {
        private readonly BattleConfiguration _configuration;
        private readonly Vector3 _origin;
        private readonly float _spacing;

        public GridFormationLayout(BattleConfiguration configuration, IUnitCatalog catalog, Vector3 origin)
        {
            if (configuration.ArmySize < 1 || configuration.Columns < 1 || configuration.ArmyGap <= 0f)
            {
                throw new ArgumentException("Army size, formation columns and army gap must be positive.");
            }

            _configuration = configuration;
            _origin = origin;
            _spacing = Mathf.Max(configuration.RowSpacing, catalog.MaximumRadius * 2f + configuration.MeleeReach);

            if (configuration.ArmyGap <= catalog.MaximumRadius * 2f + configuration.MeleeReach)
            {
                throw new ArgumentException("The army gap must exceed the largest melee contact distance.");
            }
        }

        public Vector3 GetPosition(int armyIndex, int slotIndex)
        {
            int row = slotIndex / _configuration.Columns;
            int column = slotIndex % _configuration.Columns;
            float direction = armyIndex == 0 ? 1f : -1f;
            float horizontal = (column - (_configuration.Columns - 1) * 0.5f) * _spacing;
            float depth = _configuration.ArmyGap * 0.5f + row * _spacing;
            Vector3 result = _origin + new Vector3(horizontal * direction, 0f, -depth * direction);
            return result;
        }
    }
}
