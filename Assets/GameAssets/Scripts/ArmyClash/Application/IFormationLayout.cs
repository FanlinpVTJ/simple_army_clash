using UnityEngine;

namespace SimpleArmyClash.Application
{
    public interface IFormationLayout
    {
        Vector3 GetPosition(int armyIndex, int slotIndex);
    }
}
