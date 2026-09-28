using UnityEngine;

namespace SimpleArmyClash.Configuration
{
    [CreateAssetMenu(menuName = "Army Clash/Battle", fileName = "BattleConfiguration")]
    public sealed class BattleConfiguration : ScriptableObject
    {
        [SerializeField, Min(1)]
        private int _armySize = 20;

        [SerializeField, Min(1)]
        private int _columns = 5;

        [SerializeField, Min(0.1f), Tooltip("Minimum distance between formation slots; enlarged for the largest unit.")]
        private float _rowSpacing = 2.2f;

        [SerializeField, Min(0.1f), Tooltip("Minimum distance between the two front rows.")]
        private float _armyGap = 12f;

        [SerializeField, Min(0.001f), Tooltip("Duration of one simulation step in seconds.")]
        private float _fixedStep = 1f / 30f;

        [SerializeField, Min(0f)]
        private float _meleeReach = 0.1f;

        [SerializeField, Range(0f, 1f)]
        private float _separationStrength = 0.6f;

        [SerializeField]
        private Color _firstArmyColor = new Color(1f, 0.8f, 0.15f);

        [SerializeField]
        private Color _secondArmyColor = new Color(0.65f, 0.2f, 1f);

        public int ArmySize => _armySize;
        public int Columns => _columns;
        public float RowSpacing => _rowSpacing;
        public float ArmyGap => _armyGap;
        public float FixedStep => _fixedStep;
        public float MeleeReach => _meleeReach;
        public float SeparationStrength => _separationStrength;
        public Color FirstArmyColor => _firstArmyColor;
        public Color SecondArmyColor => _secondArmyColor;
    }
}
