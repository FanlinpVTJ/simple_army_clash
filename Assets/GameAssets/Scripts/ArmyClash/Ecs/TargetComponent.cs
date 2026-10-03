using Scellecs.Morpeh;

namespace SimpleArmyClash.Ecs
{
    public struct TargetComponent : IComponent
    {
        public Entity Entity;
        public bool HasTarget;
    }
}
