using System;
using System.Collections.Generic;
using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using SimpleArmyClash.Presentation;
using SimpleArmyClash.Simulation;
using UnityEngine;

namespace SimpleArmyClash.Application
{
    public sealed class BattlePresentation : IDisposable
    {
        public event Action<int> OnUnitSelected = delegate { };

        private readonly IUnitViewFactory _factory;
        private readonly List<IUnitView> _ownedViews = new List<IUnitView>();
        private UnitWorld _units;
        private Stash<UnitViewComponent> _views;
        private SystemsGroup _systems;
        private IBattleSimulation _simulation;
        private bool _hasWorld;
        private bool _isAttached;

        public BattlePresentation(IUnitViewFactory factory)
        {
            _factory = factory;
        }

        public void Show(UnitWorld units)
        {
            Clear();
            _units = units;
            _views = units.World.GetStash<UnitViewComponent>();
            _systems = units.World.CreateSystemsGroup();
            _systems.AddSystem(new UnitViewSystem());
            _hasWorld = true;

            foreach (Entity entity in units.LivingUnits)
            {
                IUnitView view = _factory.Create(units.GetSnapshot(entity));
                view.OnSelected += HandleSelected;
                _ownedViews.Add(view);
                _views.Add(entity).View = view;
            }

            units.World.Commit();
            _systems.Initialize();
        }

        public void Attach(IBattleSimulation simulation)
        {
            Detach();
            _simulation = simulation;
            _simulation.OnUnitAttacked += HandleAttack;
            _simulation.OnUnitDied += HandleDeath;
            _isAttached = true;
        }

        public void Synchronize(float deltaTime)
        {
            if (_hasWorld && !_units.World.IsDisposed)
            {
                _systems.Update(deltaTime);
            }
        }

        public void SetSelected(int identifier)
        {
            if (!_hasWorld)
            {
                return;
            }

            foreach (Entity entity in _units.LivingUnits)
            {
                _units.Selection.Get(entity).IsSelected = _units.Units.Get(entity).Identifier == identifier;
            }
        }

        public void Clear()
        {
            Detach();

            for (int i = 0; i < _ownedViews.Count; i++)
            {
                IUnitView view = _ownedViews[i];
                view.OnSelected -= HandleSelected;
                _factory.Release(view);
            }

            _ownedViews.Clear();
            DetachWorld();
        }

        public void Dispose()
        {
            Detach();

            for (int i = 0; i < _ownedViews.Count; i++)
            {
                _ownedViews[i].OnSelected -= HandleSelected;
            }

            _ownedViews.Clear();
            DetachWorld();
        }

        private void DetachWorld()
        {
            if (!_hasWorld)
            {
                return;
            }

            if (!_units.World.IsDisposed)
            {
                _systems.Dispose();
                _views.RemoveAll();
                _units.World.Commit();
            }

            _hasWorld = false;
        }

        private void Detach()
        {
            if (!_isAttached)
            {
                return;
            }

            _simulation.OnUnitAttacked -= HandleAttack;
            _simulation.OnUnitDied -= HandleDeath;
            _isAttached = false;
        }

        private void HandleSelected(int identifier)
        {
            OnUnitSelected(identifier);
        }

        private void HandleAttack(int attackerIdentifier, Vector3 targetPosition)
        {
            if (!_hasWorld || !_units.Contains(attackerIdentifier))
            {
                return;
            }

            Entity attacker = _units.GetEntity(attackerIdentifier);

            if (_views.Has(attacker))
            {
                ref UnitViewComponent view = ref _views.Get(attacker);
                view.AttackTarget = targetPosition;
                view.HasPendingAttack = true;
            }
        }

        private void HandleDeath(int identifier)
        {
            Entity entity = _units.GetEntity(identifier);

            if (!_views.Has(entity))
            {
                return;
            }

            IUnitView view = _views.Get(entity).View;
            view.OnSelected -= HandleSelected;
            _factory.Release(view);
            _ownedViews.Remove(view);
            _views.Remove(entity);
        }
    }
}
