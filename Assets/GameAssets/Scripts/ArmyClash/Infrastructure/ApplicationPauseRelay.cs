using UnityEngine;
using Zenject;

namespace SimpleArmyClash.Infrastructure
{
    public sealed class ApplicationPauseRelay : MonoBehaviour
    {
        private BattleRunner _runner;
        private bool _isInjected;
        private bool _paused;

        private void OnApplicationPause(bool paused)
        {
            _paused = paused;

            if (_isInjected)
            {
                _runner.SetPaused(paused);
            }
        }

        [Inject]
        public void Construct(BattleRunner runner)
        {
            _runner = runner;
            _isInjected = true;
            _runner.SetPaused(_paused);
        }
    }
}
