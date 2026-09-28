using R3;

namespace SimpleArmyClash.Application
{
    public interface IBattleReadModel
    {
        ReadOnlyReactiveProperty<GamePhaseType> Phase { get; }
        ReadOnlyReactiveProperty<int> FirstArmyCount { get; }
        ReadOnlyReactiveProperty<int> SecondArmyCount { get; }
        ReadOnlyReactiveProperty<int> ElapsedSeconds { get; }
        ReadOnlyReactiveProperty<string> ResultText { get; }
        ReadOnlyReactiveProperty<int> SelectedUnitIdentifier { get; }
    }
}
