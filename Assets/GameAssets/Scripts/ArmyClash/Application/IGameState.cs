namespace SimpleArmyClash.Application
{
    public interface IGameState
    {
        GamePhaseType Phase { get; }
        void Enter();
        void Exit();
    }
}
