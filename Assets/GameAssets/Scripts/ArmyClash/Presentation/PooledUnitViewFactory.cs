using System;
using System.Collections.Generic;
using PoolsUtility;
using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;
using UnityEngine;
using Zenject;

namespace SimpleArmyClash.Presentation
{
    public sealed class PooledUnitViewFactory : IUnitViewFactory
    {
        private readonly Dictionary<string, PooledObjectPool> _pools;
        private readonly Dictionary<string, Color> _colors;
        private readonly Dictionary<PooledObject, UnitView> _views;
        private readonly Dictionary<IUnitView, ActiveView> _activeViews;
        private readonly Transform _unitsRoot;
        private readonly Color _firstArmyColor;
        private readonly Color _secondArmyColor;
        private bool _disposed;

        private readonly struct ActiveView
        {
            public UnitView View { get; }
            public PooledObjectPool Pool { get; }

            public ActiveView(UnitView view, PooledObjectPool pool)
            {
                View = view;
                Pool = pool;
            }
        }

        public PooledUnitViewFactory(UnitCatalogConfiguration catalog, BattleConfiguration battleConfiguration,
            PoolManager poolManager, Transform unitsRoot)
        {
            _unitsRoot = unitsRoot;
            _firstArmyColor = battleConfiguration.FirstArmyColor;
            _secondArmyColor = battleConfiguration.SecondArmyColor;
            _pools = new Dictionary<string, PooledObjectPool>(catalog.Shapes.Length);
            _colors = new Dictionary<string, Color>(catalog.Colors.Length);
            int capacity = battleConfiguration.ArmySize * 2;
            _views = new Dictionary<PooledObject, UnitView>(capacity * catalog.Shapes.Length);
            _activeViews = new Dictionary<IUnitView, ActiveView>(capacity);

            for (int i = 0; i < catalog.Colors.Length; i++)
            {
                UnitColorConfiguration color = catalog.Colors[i];
                _colors.Add(color.Identifier, color.VisualColor);
            }

            for (int i = 0; i < catalog.Shapes.Length; i++)
            {
                UnitShapeConfiguration shape = catalog.Shapes[i];
                string poolIdentifier = $"ArmyClash_{unitsRoot.GetInstanceID()}_{shape.Identifier}";
                PoolGroup group = new PoolGroup(poolIdentifier, shape.Prefab, capacity, capacity, PoolExpandMethods.Disabled);
                PooledObjectPool pool = poolManager.InitPool(group);
                _pools.Add(shape.Identifier, pool);
            }
        }

        public IUnitView Create(UnitState state)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PooledUnitViewFactory));
            }

            PooledObjectPool pool = _pools[state.Definition.ShapeIdentifier];
            PooledObject instance = pool.Spawn();

            if (!_views.TryGetValue(instance, out UnitView view))
            {
                view = instance.GetComponent<UnitView>();
                _views.Add(instance, view);
            }

            instance.transform.SetParent(_unitsRoot, false);
            Color bodyColor = _colors[state.Definition.ColorIdentifier];
            Color teamColor = state.ArmyIndex == 0 ? _firstArmyColor : _secondArmyColor;
            view.Configure(state, bodyColor, teamColor);
            _activeViews.Add(view, new ActiveView(view, pool));
            return view;
        }

        public void Release(IUnitView view)
        {
            if (!_activeViews.TryGetValue(view, out ActiveView activeView))
            {
                return;
            }

            view.ResetForPool();
            _activeViews.Remove(view);
            activeView.Pool.Despawn(activeView.View);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (KeyValuePair<IUnitView, ActiveView> pair in _activeViews)
            {
                UnitView view = pair.Value.View;

                if (view != null)
                {
                    UnityEngine.Object.Destroy(view.gameObject);
                }
            }

            _activeViews.Clear();
            _views.Clear();

            foreach (PooledObjectPool pool in _pools.Values)
            {
                pool.Dispose();
            }

            _pools.Clear();
        }
    }
}
