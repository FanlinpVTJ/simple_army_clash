using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Application
{
    public sealed class FormationSwapSystem : ISystem
    {
        private Filter _requests;
        private Stash<FormationSwapRequest> _swaps;
        private Stash<UnitComponent> _units;
        private Stash<PositionComponent> _positions;

        public World World { get; set; }

        public void OnAwake()
        {
            _requests = World.Filter.With<FormationSwapRequest>().With<UnitComponent>().With<PositionComponent>().Build();
            _swaps = World.GetStash<FormationSwapRequest>();
            _units = World.GetStash<UnitComponent>();
            _positions = World.GetStash<PositionComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (Entity entity in _requests)
            {
                Entity other = _swaps.Get(entity).Other;

                if (World.Has(other) && _units.Has(other) && _positions.Has(other)
                    && _units.Get(entity).ArmyIndex == _units.Get(other).ArmyIndex)
                {
                    ref PositionComponent first = ref _positions.Get(entity);
                    ref PositionComponent second = ref _positions.Get(other);
                    Vector3 position = first.Value;
                    first.Value = second.Value;
                    second.Value = position;
                }

                _swaps.Remove(entity);
            }
        }

        public void Dispose()
        {
        }
    }
}
