using System;
using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [CreateAssetMenu(menuName = "Army Clash/Unit Catalog", fileName = "UnitCatalog")]
    public sealed class UnitCatalogConfiguration : ScriptableObject
    {
        [SerializeField]
        private SerializedStatisticsModifier _baseStatistics = new SerializedStatisticsModifier(100, 10, 10f, 1f);

        [SerializeField]
        private UnitShapeConfiguration[] _shapes = Array.Empty<UnitShapeConfiguration>();

        [SerializeField]
        private UnitColorConfiguration[] _colors = Array.Empty<UnitColorConfiguration>();

        [SerializeField]
        private UnitSizeConfiguration[] _sizes = Array.Empty<UnitSizeConfiguration>();

        public SerializedStatisticsModifier BaseStatistics => _baseStatistics;
        public UnitShapeConfiguration[] Shapes => _shapes;
        public UnitColorConfiguration[] Colors => _colors;
        public UnitSizeConfiguration[] Sizes => _sizes;
    }
}
