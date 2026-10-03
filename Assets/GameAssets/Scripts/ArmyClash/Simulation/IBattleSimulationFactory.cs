using SimpleArmyClash.Ecs;

namespace SimpleArmyClash.Simulation
{
    public interface IBattleSimulationFactory
    {
        IBattleSimulation Create(UnitWorld units);
    }
}
