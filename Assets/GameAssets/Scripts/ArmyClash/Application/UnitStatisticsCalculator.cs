using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public sealed class UnitStatisticsCalculator : IUnitStatisticsCalculator
    {
        public UnitStatistics Calculate(SerializedStatisticsModifier baseStatistics, SerializedStatisticsModifier shape,
            SerializedStatisticsModifier color, SerializedStatisticsModifier size)
        {
            UnitStatistics result = new UnitStatistics(
                baseStatistics.Health + shape.Health + color.Health + size.Health,
                baseStatistics.AttackDamage + shape.AttackDamage + color.AttackDamage + size.AttackDamage,
                baseStatistics.MovementSpeed + shape.MovementSpeed + color.MovementSpeed + size.MovementSpeed,
                baseStatistics.AttackInterval + shape.AttackInterval + color.AttackInterval + size.AttackInterval);
            return result;
        }
    }
}
