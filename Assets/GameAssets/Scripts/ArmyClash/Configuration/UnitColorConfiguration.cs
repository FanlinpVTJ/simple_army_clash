using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [CreateAssetMenu(menuName = "Army Clash/Unit Color", fileName = "UnitColor")]
    public sealed class UnitColorConfiguration : ScriptableObject
    {
        [SerializeField, Tooltip("Unique stable color identifier, for example Blue.")]
        private string _identifier = "Blue";

        [SerializeField]
        private Color _visualColor = Color.blue;

        [SerializeField]
        private SerializedStatisticsModifier _statisticsModifier = new SerializedStatisticsModifier(0, -15, 10f, 4f);

        public string Identifier => _identifier;
        public Color VisualColor => _visualColor;
        public SerializedStatisticsModifier StatisticsModifier => _statisticsModifier;
    }
}
