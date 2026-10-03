using System;
using SimpleArmyClash.Domain;
using UnityEngine;

namespace SimpleArmyClash.Presentation
{
    public interface IUnitView
    {
        event Action<int> OnSelected;

        int Identifier { get; }

        void Configure(UnitState state, Color bodyColor, Color teamColor);
        void Synchronize(Vector3 position, float visualScale);
        void PlayAttack(Vector3 targetPosition);
        void SetSelected(bool selected);
        void ResetForPool();
    }
}
