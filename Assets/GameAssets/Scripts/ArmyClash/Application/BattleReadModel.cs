using System;
using R3;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public sealed class BattleReadModel : IBattleReadModel, IDisposable
    {
        private readonly ReactiveProperty<GamePhaseType> _phase = new ReactiveProperty<GamePhaseType>(GamePhaseType.Initializing);
        private readonly ReactiveProperty<int> _firstArmyCount = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _secondArmyCount = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<int> _elapsedSeconds = new ReactiveProperty<int>(0);
        private readonly ReactiveProperty<string> _resultText = new ReactiveProperty<string>(string.Empty);
        private readonly ReactiveProperty<int> _selectedUnitIdentifier = new ReactiveProperty<int>(-1);

        public ReadOnlyReactiveProperty<GamePhaseType> Phase => _phase;
        public ReadOnlyReactiveProperty<int> FirstArmyCount => _firstArmyCount;
        public ReadOnlyReactiveProperty<int> SecondArmyCount => _secondArmyCount;
        public ReadOnlyReactiveProperty<int> ElapsedSeconds => _elapsedSeconds;
        public ReadOnlyReactiveProperty<string> ResultText => _resultText;
        public ReadOnlyReactiveProperty<int> SelectedUnitIdentifier => _selectedUnitIdentifier;

        public void SetPhase(GamePhaseType phase)
        {
            _phase.Value = phase;
        }

        public void SetCounts(int firstArmyCount, int secondArmyCount)
        {
            _firstArmyCount.Value = firstArmyCount;
            _secondArmyCount.Value = secondArmyCount;
        }

        public void SetElapsedTime(float elapsedTime)
        {
            _elapsedSeconds.Value = (int)elapsedTime;
        }

        public void SelectUnit(int identifier)
        {
            _selectedUnitIdentifier.Value = identifier;
        }

        public void SetResult(BattleResult result)
        {
            string armyName = result.WinnerArmyIndex == 0 ? "A" : "B";
            _resultText.Value = $"Победила армия {armyName}. Выжило: {result.Survivors}. Время: {result.ElapsedTime:0.0} с.";
        }

        public void Dispose()
        {
            _phase.Dispose();
            _firstArmyCount.Dispose();
            _secondArmyCount.Dispose();
            _elapsedSeconds.Dispose();
            _resultText.Dispose();
            _selectedUnitIdentifier.Dispose();
        }
    }
}
