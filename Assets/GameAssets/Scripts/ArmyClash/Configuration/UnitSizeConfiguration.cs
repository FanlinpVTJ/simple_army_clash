using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [CreateAssetMenu(menuName = "Army Clash/Unit Size", fileName = "UnitSize")]
    public sealed class UnitSizeConfiguration : ScriptableObject
    {
        [SerializeField, Tooltip("Unique stable size identifier, for example Small or Big.")]
        private string _identifier = "Small";

        [SerializeField, Min(0.01f)]
        private float _scale = 0.8f;

        [SerializeField]
        private SerializedStatisticsModifier _statisticsModifier = new SerializedStatisticsModifier(-50, 0, 0f, 0f);

        public string Identifier => _identifier;
        public float Scale => _scale;
        public SerializedStatisticsModifier StatisticsModifier => _statisticsModifier;
    }
}
