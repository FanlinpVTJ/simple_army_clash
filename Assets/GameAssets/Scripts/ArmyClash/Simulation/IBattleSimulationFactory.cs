using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Simulation
{
    public interface IBattleSimulationFactory
    {
        IBattleSimulation Create(UnitState[] units);
    }
}
