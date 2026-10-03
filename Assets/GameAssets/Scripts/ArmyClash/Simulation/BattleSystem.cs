using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;

namespace SimpleArmyClash.Simulation
{
    public abstract class BattleSystem : IBattleSystem
    {
        public World World { get; set; }

        protected BattleSimulationState State { get; }
        protected UnitWorld Units => State.Units;

        protected BattleSystem(BattleSimulationState state)
        {
            State = state;
        }

        public abstract void OnAwake();
        public abstract void OnUpdate(float deltaTime);

        public virtual void Dispose()
        {
        }
    }
}
