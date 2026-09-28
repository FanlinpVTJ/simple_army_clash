#nullable enable

using System;
using R3;
using SimpleArmyClash.Application;

namespace SimpleArmyClash.UI
{
    public sealed class BattleViewModel : IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private readonly ReactiveProperty<string> _elapsedTimeText = new ReactiveProperty<string>(string.Empty);

        public ReadOnlyReactiveProperty<int> FirstArmyCount { get; }
        public ReadOnlyReactiveProperty<int> SecondArmyCount { get; }
        public ReadOnlyReactiveProperty<string> ElapsedTimeText => _elapsedTimeText;

        public BattleViewModel(IBattleReadModel battleReadModel)
        {
            FirstArmyCount = battleReadModel.FirstArmyCount;
            SecondArmyCount = battleReadModel.SecondArmyCount;
            _subscriptions.Add(battleReadModel.ElapsedSeconds.Subscribe(UpdateElapsedTime));
            _subscriptions.Add(_elapsedTimeText);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        private void UpdateElapsedTime(int elapsedSeconds)
        {
            int minutes = elapsedSeconds / 60;
            int seconds = elapsedSeconds % 60;
            _elapsedTimeText.Value = $"Время боя: {minutes:00}:{seconds:00}";
        }
    }
}
