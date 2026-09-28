#nullable enable

using R3;
using TMPro;
using UnityEngine;
using WindowsManager;
using Zenject;

namespace SimpleArmyClash.UI
{
    public sealed class BattleWindow : Window
    {
        [Header("Состояние сражения")]
        [SerializeField, Tooltip("Обязательный текст количества оставшихся юнитов первой армии.")]
        private TMP_Text _firstArmyCountText = null!;

        [SerializeField, Tooltip("Обязательный текст количества оставшихся юнитов второй армии.")]
        private TMP_Text _secondArmyCountText = null!;

        [SerializeField, Tooltip("Обязательный текст времени сражения.")]
        private TMP_Text _elapsedTimeText = null!;

        private readonly CompositeDisposable _bindings = new CompositeDisposable();
        private BattleViewModel _viewModel = null!;

        private void OnDestroy()
        {
            _bindings.Dispose();
        }

        [Inject]
        public void Construct(BattleViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        protected override void OpenInner()
        {
            base.OpenInner();
            _bindings.Clear();
            _bindings.Add(_viewModel.FirstArmyCount.SubscribeToText(_firstArmyCountText, count => $"Армия A: {count}"));
            _bindings.Add(_viewModel.SecondArmyCount.SubscribeToText(_secondArmyCountText, count => $"Армия B: {count}"));
            _bindings.Add(_viewModel.ElapsedTimeText.SubscribeToText(_elapsedTimeText));
        }

        protected override void CloseInner()
        {
            _bindings.Clear();
            base.CloseInner();
        }
    }
}
