using System;
using R3;
using SimpleArmyClash.Application;
using WindowsManager;
using Zenject;

namespace SimpleArmyClash.Infrastructure
{
    public sealed class GameWindowsController : IInitializable, IDisposable
    {
        private readonly IWindowsManager _windows;
        private readonly IBattleReadModel _readModel;
        private readonly WindowData _mainMenuWindow;
        private readonly WindowData _battleWindow;
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private GamePhaseType _displayedPhase = GamePhaseType.Initializing;

        public GameWindowsController(IWindowsManager windows, IBattleReadModel readModel,
            WindowData mainMenuWindow, WindowData battleWindow)
        {
            _windows = windows;
            _readModel = readModel;
            _mainMenuWindow = mainMenuWindow;
            _battleWindow = battleWindow;
        }

        public void Initialize()
        {
            _subscriptions.Add(_readModel.Phase.Subscribe(ShowPhase));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }

        private void ShowPhase(GamePhaseType phase)
        {
            if (phase == _displayedPhase)
            {
                return;
            }

            if (phase == GamePhaseType.Preparing)
            {
                if (_displayedPhase == GamePhaseType.Running)
                {
                    _windows.CloseWindow(_battleWindow);
                }

                _windows.OpenWindow(_mainMenuWindow);
                _displayedPhase = phase;
            }
            else if (phase == GamePhaseType.Running)
            {
                if (_displayedPhase == GamePhaseType.Preparing)
                {
                    _windows.CloseWindow(_mainMenuWindow);
                }

                _windows.OpenWindow(_battleWindow);
                _displayedPhase = phase;
            }
        }
    }
}
