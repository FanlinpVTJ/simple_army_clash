using Scellecs.Morpeh;
using UnityEngine;

namespace SimpleArmyClash.Ecs
{
    public struct PositionComponent : IComponent
    {
        public Vector3 Value;
        public float Radius;
    }
}
