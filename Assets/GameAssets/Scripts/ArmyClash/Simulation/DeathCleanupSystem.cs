using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;

namespace SimpleArmyClash.Simulation
{
    public sealed class DeathCleanupSystem : BattleSystem
    {
        private Filter _filter;

        public DeathCleanupSystem(BattleSimulationState state) : base(state)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<AliveComponent>().With<HealthComponent>().With<TargetComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (Entity entity in _filter)
            {
                if (Units.Health.Get(entity).Current > 0)
                {
                    continue;
                }

                Units.Alive.Remove(entity);
                Units.Targets.Get(entity).HasTarget = false;
                State.RecordDeath(entity);
            }
        }
    }
}
