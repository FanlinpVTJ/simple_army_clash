using System.Collections.Generic;

namespace SimpleArmyClash.Application
{
    public sealed class GameStateMachine
    {
        private readonly Dictionary<GamePhaseType, IGameState> _states = new Dictionary<GamePhaseType, IGameState>();
        private readonly BattleReadModel _readModel;
        private IGameState _currentState;
        private bool _hasCurrentState;

        public GameStateMachine(List<IGameState> states, BattleReadModel readModel)
        {
            _readModel = readModel;

            foreach (IGameState state in states)
            {
                _states.Add(state.Phase, state);
            }
        }

        public void ChangeState(GamePhaseType phase)
        {
            _readModel.SetPhase(_hasCurrentState ? GamePhaseType.Finishing : GamePhaseType.Initializing);
            Stop();
            _currentState = _states[phase];
            _hasCurrentState = true;
            _currentState.Enter();
            _readModel.SetPhase(phase);
        }

        public void Stop()
        {
            if (!_hasCurrentState)
            {
                return;
            }

            _currentState.Exit();
            _hasCurrentState = false;
        }
    }
}
