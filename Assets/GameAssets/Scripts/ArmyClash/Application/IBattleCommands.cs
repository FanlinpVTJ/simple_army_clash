namespace SimpleArmyClash.Application
{
    public interface IBattleCommands
    {
        void RandomizeArmies();
        void StartBattle();
        void SelectUnit(int unitIdentifier);
    }
}
