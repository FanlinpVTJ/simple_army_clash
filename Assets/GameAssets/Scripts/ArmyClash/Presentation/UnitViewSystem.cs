using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Presentation
{
    public sealed class UnitViewSystem : ISystem
    {
        private const float ATTACK_DURATION = 0.18f;
        private const float ATTACK_PULSE_SCALE = 0.12f;

        private Filter _filter;
        private Stash<UnitViewComponent> _views;
        private Stash<PositionComponent> _positions;
        private Stash<SelectionComponent> _selection;

        public World World { get; set; }

        public void OnAwake()
        {
            _filter = World.Filter.With<UnitViewComponent>().With<PositionComponent>().With<SelectionComponent>().Build();
            _views = World.GetStash<UnitViewComponent>();
            _positions = World.GetStash<PositionComponent>();
            _selection = World.GetStash<SelectionComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (Entity entity in _filter)
            {
                ref UnitViewComponent component = ref _views.Get(entity);

                if (component.HasPendingAttack)
                {
                    component.View.PlayAttack(component.AttackTarget);
                    component.AttackTimeRemaining = ATTACK_DURATION;
                    component.HasPendingAttack = false;
                }

                component.AttackTimeRemaining = Mathf.Max(0f, component.AttackTimeRemaining - deltaTime);
                float progress = component.AttackTimeRemaining / ATTACK_DURATION;
                float pulse = 1f + Mathf.Sin(progress * Mathf.PI) * ATTACK_PULSE_SCALE;
                component.View.Synchronize(_positions.Get(entity).Value, pulse);
                component.View.SetSelected(_selection.Get(entity).IsSelected);
            }
        }

        public void Dispose()
        {
        }
    }
}
