#nullable enable

using System;
using R3;
using SimpleArmyClash.Application;

namespace SimpleArmyClash.UI
{
    public sealed class MainMenuViewModel : IDisposable
    {
        private const string DEFAULT_FORMATION_HINT = "Выберите двух юнитов одной армии, чтобы поменять их местами.";
        private const string SELECTED_FORMATION_HINT = "Юнит выбран. Выберите второго юнита той же армии для обмена.";

        private readonly IBattleCommands _battleCommands;
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private readonly ReactiveProperty<bool> _canEditArmies = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<string> _formationHint = new ReactiveProperty<string>(DEFAULT_FORMATION_HINT);

        public ReactiveCommand RandomizeCommand { get; }
        public ReactiveCommand StartCommand { get; }
        public ReadOnlyReactiveProperty<bool> CanEditArmies => _canEditArmies;
        public ReadOnlyReactiveProperty<int> FirstArmyCount { get; }
        public ReadOnlyReactiveProperty<int> SecondArmyCount { get; }
        public ReadOnlyReactiveProperty<string> ResultText { get; }
        public ReadOnlyReactiveProperty<string> FormationHint => _formationHint;

        public MainMenuViewModel(IBattleCommands battleCommands, IBattleReadModel battleReadModel)
        {
            _battleCommands = battleCommands;
            FirstArmyCount = battleReadModel.FirstArmyCount;
            SecondArmyCount = battleReadModel.SecondArmyCount;
            ResultText = battleReadModel.ResultText;
            RandomizeCommand = new ReactiveCommand(_canEditArmies, false);
            StartCommand = new ReactiveCommand(_canEditArmies, false);

            _subscriptions.Add(RandomizeCommand.Subscribe(_ => RandomizeArmies()));
            _subscriptions.Add(StartCommand.Subscribe(_ => StartBattle()));
            _subscriptions.Add(battleReadModel.Phase.Subscribe(UpdatePhase));
            _subscriptions.Add(battleReadModel.SelectedUnitIdentifier.Subscribe(UpdateFormationHint));
            _subscriptions.Add(RandomizeCommand);
            _subscriptions.Add(StartCommand);
            _subscriptions.Add(_canEditArmies);
            _subscriptions.Add(_formationHint);
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        private void RandomizeArmies()
        {
            if (!_canEditArmies.Value)
            {
                return;
            }

            _battleCommands.RandomizeArmies();
        }

        private void StartBattle()
        {
            if (!_canEditArmies.Value)
            {
                return;
            }

            _battleCommands.StartBattle();
        }

        private void UpdatePhase(GamePhaseType phase)
        {
            _canEditArmies.Value = phase == GamePhaseType.Preparing;
        }

        private void UpdateFormationHint(int selectedUnitIdentifier)
        {
            _formationHint.Value = selectedUnitIdentifier < 0 ? DEFAULT_FORMATION_HINT : SELECTED_FORMATION_HINT;
        }
    }
}
