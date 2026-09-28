using SimpleArmyClash.Presentation;
using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [CreateAssetMenu(menuName = "Army Clash/Unit Shape", fileName = "UnitShape")]
    public sealed class UnitShapeConfiguration : ScriptableObject
    {
        [SerializeField, Tooltip("Unique stable shape identifier, for example Cube or Sphere.")]
        private string _identifier = "Cube";

        [SerializeField, Tooltip("Required prefab with UnitView on its root. Drag the unit prefab here.")]
        private UnitView _prefab;

        [SerializeField]
        private SerializedStatisticsModifier _statisticsModifier = new SerializedStatisticsModifier(100, 10, 0f, 0f);

        [SerializeField, Min(0.01f), Tooltip("Horizontal collision radius at scale one.")]
        private float _radius = 0.5f;

        public string Identifier => _identifier;
        public UnitView Prefab => _prefab;
        public SerializedStatisticsModifier StatisticsModifier => _statisticsModifier;
        public float Radius => _radius;
    }
}
