using Scellecs.Morpeh;

namespace SimpleArmyClash.Ecs
{
    public struct AttackComponent : IComponent
    {
        public int Damage;
        public float Interval;
        public float ReadyAt;
        public int Priority;
    }
}
