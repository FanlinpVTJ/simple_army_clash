using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public interface IUnitStatisticsCalculator
    {
        UnitStatistics Calculate(SerializedStatisticsModifier baseStatistics, SerializedStatisticsModifier shape,
            SerializedStatisticsModifier color, SerializedStatisticsModifier size);
    }
}
