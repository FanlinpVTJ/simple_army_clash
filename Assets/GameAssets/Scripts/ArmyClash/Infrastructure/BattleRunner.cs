using System;
using SimpleArmyClash.Application;
using SimpleArmyClash.Configuration;
using UnityEngine;
using Zenject;

namespace SimpleArmyClash.Infrastructure
{
    public sealed class BattleRunner : ITickable, IDisposable
    {
        private const int MAXIMUM_STEPS_PER_FRAME = 8;

        private readonly BattleSessionService _session;
        private readonly GameFlowService _flow;
        private readonly BattleReadModel _readModel;
        private readonly BattlePresentation _presentation;
        private readonly float _fixedStep;
        private float _accumulatedTime;
        private bool _hasFocus;
        private bool _isPaused;
        private bool _discardNextFrame;

        public BattleRunner(BattleSessionService session, GameFlowService flow, BattleReadModel readModel,
            BattlePresentation presentation, BattleConfiguration configuration)
        {
            if (configuration.FixedStep <= 0f || float.IsNaN(configuration.FixedStep) ||
                float.IsInfinity(configuration.FixedStep))
            {
                throw new ArgumentOutOfRangeException(nameof(configuration), "The simulation step must be finite and positive.");
            }

            _session = session;
            _flow = flow;
            _readModel = readModel;
            _presentation = presentation;
            _fixedStep = configuration.FixedStep;
            _hasFocus = UnityEngine.Application.isFocused;
            UnityEngine.Application.focusChanged += HandleFocusChanged;
        }

        public void Tick()
        {
            if (!_hasFocus || _isPaused)
            {
                _accumulatedTime = 0f;
                _presentation.Synchronize(0f);
                return;
            }

            if (_discardNextFrame)
            {
                _discardNextFrame = false;
                _accumulatedTime = 0f;
                _presentation.Synchronize(0f);
                return;
            }

            if (_readModel.Phase.CurrentValue != GamePhaseType.Running || !_session.HasSession)
            {
                _accumulatedTime = 0f;
                _presentation.Synchronize(0f);
                return;
            }

            _accumulatedTime = Mathf.Min(_accumulatedTime + Time.deltaTime, _fixedStep * MAXIMUM_STEPS_PER_FRAME);
            int completedSteps = 0;

            while (_accumulatedTime >= _fixedStep && completedSteps < MAXIMUM_STEPS_PER_FRAME)
            {
                _session.Simulation.Step(_fixedStep);
                _accumulatedTime -= _fixedStep;
                completedSteps++;

                if (_session.HasResult)
                {
                    _flow.FinishBattle(_session.Result);
                    _accumulatedTime = 0f;
                    _presentation.Synchronize(0f);
                    return;
                }
            }

            _readModel.SetCounts(_session.Simulation.GetAliveCount(0), _session.Simulation.GetAliveCount(1));
            _readModel.SetElapsedTime(_session.Simulation.ElapsedTime);
            _presentation.Synchronize(Time.deltaTime);
        }

        public void SetPaused(bool paused)
        {
            _isPaused = paused;
            _accumulatedTime = 0f;
            _discardNextFrame = true;
        }

        public void Dispose()
        {
            UnityEngine.Application.focusChanged -= HandleFocusChanged;
        }

        private void HandleFocusChanged(bool hasFocus)
        {
            _hasFocus = hasFocus;
            _accumulatedTime = 0f;
            _discardNextFrame = true;
        }
    }
}
