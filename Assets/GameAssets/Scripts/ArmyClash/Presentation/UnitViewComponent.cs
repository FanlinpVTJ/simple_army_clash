using Scellecs.Morpeh;
using UnityEngine;

namespace SimpleArmyClash.Presentation
{
    public struct UnitViewComponent : IComponent
    {
        public IUnitView View;
        public float AttackTimeRemaining;
        public Vector3 AttackTarget;
        public bool HasPendingAttack;
    }
}
