using System.Collections.Generic;
using Scellecs.Morpeh;
using SimpleArmyClash.Ecs;
using UnityEngine;

namespace SimpleArmyClash.Simulation
{
    public interface IUnitSpatialIndex
    {
        float MaximumRadius { get; }

        void Rebuild(UnitWorld units, float meleeReach);
        Entity FindNearestEnemy(Vector3 position, int armyIndex, out bool hasTarget);
        void CollectNeighbors(Vector3 position, float radius, List<Entity> results);
    }
}
