using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;

namespace SimpleArmyClash.Simulation
{
    public sealed class TargetSelectionSystem : BattleSystem
    {
        private readonly ITargetSelectionStrategy _strategy;
        private Filter _filter;

        public TargetSelectionSystem(BattleSimulationState state, ITargetSelectionStrategy strategy) : base(state)
        {
            _strategy = strategy;
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<UnitComponent>().With<AliveComponent>()
                .With<HealthComponent>().With<PositionComponent>().With<TargetComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (Entity entity in _filter)
            {
                ref TargetComponent target = ref Units.Targets.Get(entity);

                if (Units.Health.Get(entity).Current <= 0)
                {
                    target.HasTarget = false;
                    continue;
                }

                target.Entity = _strategy.SelectTarget(entity, Units, out bool hasTarget);
                target.HasTarget = hasTarget;
            }
        }
    }
}
