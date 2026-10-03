using Scellecs.Morpeh;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Ecs
{
    public struct UnitComponent : IComponent
    {
        public int Identifier;
        public int ArmyIndex;
        public UnitDefinition Definition;
    }
}
