using Scellecs.Morpeh;
using UnityEngine;

namespace SimpleArmyClash.Ecs
{
    public struct MovementComponent : IComponent
    {
        public float Speed;
        public Vector3 NextPosition;
    }
}
