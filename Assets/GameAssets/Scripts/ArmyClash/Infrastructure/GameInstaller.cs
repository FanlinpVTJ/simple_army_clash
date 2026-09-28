using System;
using PoolsUtility;
using SimpleArmyClash.Application;
using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Presentation;
using SimpleArmyClash.Simulation;
using SimpleArmyClash.UI;
using UnityEngine;
using WindowsManager;
using Zenject;

namespace SimpleArmyClash.Infrastructure
{
    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Game configuration")]
        [SerializeField]
        private UnitCatalogConfiguration _unitCatalog;

        [SerializeField]
        private BattleConfiguration _battleConfiguration;

        [Header("Battlefield")]
        [SerializeField, Tooltip("Required scene transform with unit scale, used as parent for unit instances.")]
        private Transform _unitsRoot;

        [SerializeField, Tooltip("Required marker. Its position is the center of the horizontal XZ battlefield.")]
        private Transform _battlefieldOrigin;

        [Header("Windows")]
        [SerializeField]
        private WindowData _mainMenuWindow;

        [SerializeField]
        private WindowData _battleWindow;

        [Header("Generation")]
        [SerializeField]
        private bool _useFixedSeed;

        [SerializeField]
        private int _randomSeed = 12345;

        public override void InstallBindings()
        {
            BindConfiguration();
            BindPreparation();
            BindSimulation();
            BindPresentation();
            BindApplication();
            BindInterface();
            BindLifecycleOrder();
        }

        private void BindConfiguration()
        {
            Container.BindInstance(_unitCatalog);
            Container.BindInstance(_battleConfiguration);
            BattleSimulationSettings settings = new BattleSimulationSettings(
                _battleConfiguration.MeleeReach, _battleConfiguration.SeparationStrength);
            Container.BindInstance(settings);
            int seed = _useFixedSeed ? _randomSeed : Environment.TickCount;
            Container.Bind<IRandomSource>().To<SeededRandomSource>().AsSingle().WithArguments(seed);
            Container.Bind<IUnitStatisticsCalculator>().To<UnitStatisticsCalculator>().AsSingle();
            Container.Bind<IUnitCatalog>().To<UnitCatalog>().AsSingle();
        }

        private void BindPreparation()
        {
            Container.Bind<IArmyGenerator>().To<ArmyGenerator>().AsSingle();
            Container.Bind<IFormationLayout>().To<GridFormationLayout>().AsSingle().WithArguments(_battlefieldOrigin.position);
            Container.Bind<BattlePreparationService>().AsSingle();
        }

        private void BindSimulation()
        {
            Container.Bind<ITargetSelectionStrategy>().To<NearestEnemyTargetSelectionStrategy>().AsSingle();
            Container.Bind<IBattleSimulationFactory>().To<BattleSimulationFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleSessionService>().AsSingle();
        }

        private void BindPresentation()
        {
            Container.Bind<PoolManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PooledUnitViewFactory>().AsSingle().WithArguments(_unitsRoot);
            Container.BindInterfacesAndSelfTo<BattlePresentation>().AsSingle();
        }

        private void BindApplication()
        {
            Container.BindInterfacesAndSelfTo<BattleReadModel>().AsSingle();
            Container.Bind<IGameState>().To<PreparingGameState>().AsSingle();
            Container.Bind<IGameState>().To<RunningGameState>().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameFlowService>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleRunner>().AsSingle();
        }

        private void BindInterface()
        {
            Container.BindInterfacesAndSelfTo<global::WindowsManager.WindowsManager>().AsSingle().WithArguments<GameObject>(null);
            Container.BindInterfacesAndSelfTo<MainMenuViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<BattleViewModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameWindowsController>().AsSingle().WithArguments(_mainMenuWindow, _battleWindow);
        }

        private void BindLifecycleOrder()
        {
            Container.BindExecutionOrder<BattleReadModel>(-100);
            Container.BindExecutionOrder<global::WindowsManager.WindowsManager>(-80);
            Container.BindExecutionOrder<PooledUnitViewFactory>(-60);
            Container.BindExecutionOrder<BattleSessionService>(-50);
            Container.BindExecutionOrder<BattlePresentation>(-40);
            Container.BindExecutionOrder<MainMenuViewModel>(-30);
            Container.BindExecutionOrder<BattleViewModel>(-30);
            Container.BindExecutionOrder<GameWindowsController>(-20);
            Container.BindExecutionOrder<GameFlowService>(0);
            Container.BindExecutionOrder<BattleRunner>(10);
        }
    }
}
