using System;
using SimpleArmyClash.Domain;
using Zenject;

namespace SimpleArmyClash.Application
{
    public sealed class GameFlowService : IBattleCommands, IInitializable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly BattlePreparationService _preparation;
        private readonly BattlePresentation _presentation;
        private readonly BattleReadModel _readModel;

        public GameFlowService(GameStateMachine stateMachine, BattlePreparationService preparation,
            BattlePresentation presentation, BattleReadModel readModel)
        {
            _stateMachine = stateMachine;
            _preparation = preparation;
            _presentation = presentation;
            _readModel = readModel;
        }

        public void Initialize()
        {
            _presentation.OnUnitSelected += SelectUnit;
            _stateMachine.ChangeState(GamePhaseType.Preparing);
        }

        public void RandomizeArmies()
        {
            if (_readModel.Phase.CurrentValue != GamePhaseType.Preparing)
            {
                return;
            }

            _stateMachine.ChangeState(GamePhaseType.Preparing);
        }

        public void StartBattle()
        {
            if (_readModel.Phase.CurrentValue != GamePhaseType.Preparing)
            {
                return;
            }

            _stateMachine.ChangeState(GamePhaseType.Running);
        }

        public void SelectUnit(int unitIdentifier)
        {
            if (_readModel.Phase.CurrentValue != GamePhaseType.Preparing
                || !_preparation.Units.Contains(unitIdentifier))
            {
                return;
            }

            int selectedIdentifier = _readModel.SelectedUnitIdentifier.CurrentValue;

            if (selectedIdentifier == unitIdentifier)
            {
                UpdateSelection(-1);
                return;
            }

            if (selectedIdentifier >= 0 && _preparation.TrySwap(selectedIdentifier, unitIdentifier))
            {
                _presentation.Synchronize(0f);
                UpdateSelection(-1);
                return;
            }

            UpdateSelection(unitIdentifier);
        }

        public void FinishBattle(BattleResult result)
        {
            if (_readModel.Phase.CurrentValue != GamePhaseType.Running)
            {
                return;
            }

            _readModel.SetResult(result);
            _stateMachine.ChangeState(GamePhaseType.Preparing);
        }

        public void Dispose()
        {
            _presentation.OnUnitSelected -= SelectUnit;
            _presentation.Dispose();
            _stateMachine.Dispose();
        }

        private void UpdateSelection(int identifier)
        {
            _readModel.SelectUnit(identifier);
            _presentation.SetSelected(identifier);
        }
    }
}
