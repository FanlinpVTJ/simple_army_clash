#nullable enable

using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WindowsManager;
using Zenject;

namespace SimpleArmyClash.UI
{
    public sealed class MainMenuWindow : Window
    {
        [Header("Кнопки")]
        [SerializeField, Tooltip("Обязательная кнопка случайной генерации обеих армий.")]
        private Button _randomizeButton = null!;

        [SerializeField, Tooltip("Обязательная кнопка начала сражения.")]
        private Button _startButton = null!;

        [Header("Состояние армий")]
        [SerializeField, Tooltip("Обязательный текст количества юнитов первой армии.")]
        private TMP_Text _firstArmyCountText = null!;

        [SerializeField, Tooltip("Обязательный текст количества юнитов второй армии.")]
        private TMP_Text _secondArmyCountText = null!;

        [SerializeField, Tooltip("Обязательный текст результата последнего сражения.")]
        private TMP_Text _resultText = null!;

        [SerializeField, Tooltip("Обязательная подсказка перестановки юнитов перед сражением.")]
        private TMP_Text _formationHintText = null!;

        private readonly CompositeDisposable _bindings = new CompositeDisposable();
        private MainMenuViewModel _viewModel = null!;

        private void OnDestroy()
        {
            _bindings.Dispose();
        }

        [Inject]
        public void Construct(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        protected override void OpenInner()
        {
            base.OpenInner();
            _bindings.Clear();
            _bindings.Add(_randomizeButton.OnClickAsObservable().Subscribe(_ => _viewModel.RandomizeCommand.Execute(Unit.Default)));
            _bindings.Add(_startButton.OnClickAsObservable().Subscribe(_ => _viewModel.StartCommand.Execute(Unit.Default)));
            _bindings.Add(_viewModel.CanEditArmies.SubscribeToInteractable(_randomizeButton));
            _bindings.Add(_viewModel.CanEditArmies.SubscribeToInteractable(_startButton));
            _bindings.Add(_viewModel.FirstArmyCount.SubscribeToText(_firstArmyCountText, count => $"Армия A: {count}"));
            _bindings.Add(_viewModel.SecondArmyCount.SubscribeToText(_secondArmyCountText, count => $"Армия B: {count}"));
            _bindings.Add(_viewModel.ResultText.SubscribeToText(_resultText));
            _bindings.Add(_viewModel.FormationHint.SubscribeToText(_formationHintText));
        }

        protected override void CloseInner()
        {
            _bindings.Clear();
            _randomizeButton.interactable = false;
            _startButton.interactable = false;
            base.CloseInner();
        }
    }
}
