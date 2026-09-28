using System;
using System.Collections.Generic;
using SimpleArmyClash.Domain;
using SimpleArmyClash.Presentation;
using SimpleArmyClash.Simulation;

namespace SimpleArmyClash.Application
{
    public sealed class BattlePresentation : IDisposable
    {
        public event Action<int> OnUnitSelected = delegate { };

        private readonly IUnitViewFactory _factory;
        private readonly Dictionary<int, IUnitView> _views = new Dictionary<int, IUnitView>();
        private UnitState[] _units = Array.Empty<UnitState>();
        private IBattleSimulation _simulation;
        private bool _isAttached;

        public BattlePresentation(IUnitViewFactory factory)
        {
            _factory = factory;
        }

        public void Show(UnitState[] units)
        {
            Clear();
            _units = units;

            foreach (UnitState unit in units)
            {
                IUnitView view = _factory.Create(unit);
                view.OnSelected += HandleSelected;
                _views.Add(unit.Identifier, view);
            }
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
            foreach (KeyValuePair<int, IUnitView> entry in _views)
            {
                entry.Value.Synchronize(_units[entry.Key], deltaTime);
            }
        }

        public void SetSelected(int identifier)
        {
            foreach (KeyValuePair<int, IUnitView> entry in _views)
            {
                entry.Value.SetSelected(entry.Key == identifier);
            }
        }

        public void Clear()
        {
            Detach();

            foreach (KeyValuePair<int, IUnitView> entry in _views)
            {
                entry.Value.OnSelected -= HandleSelected;
                _factory.Release(entry.Value);
            }

            _views.Clear();
            _units = Array.Empty<UnitState>();
        }

        public void Dispose()
        {
            Detach();

            foreach (KeyValuePair<int, IUnitView> entry in _views)
            {
                entry.Value.OnSelected -= HandleSelected;
            }

            _views.Clear();
            _units = Array.Empty<UnitState>();
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

        private void HandleAttack(int attackerIdentifier, int targetIdentifier)
        {
            if (_views.TryGetValue(attackerIdentifier, out IUnitView view))
            {
                view.PlayAttack(_units[targetIdentifier].Position);
            }
        }

        private void HandleDeath(int identifier)
        {
            if (!_views.TryGetValue(identifier, out IUnitView view))
            {
                return;
            }

            view.OnSelected -= HandleSelected;
            _factory.Release(view);
            _views.Remove(identifier);
        }
    }
}
