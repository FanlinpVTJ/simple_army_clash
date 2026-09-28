namespace SimpleArmyClash.Domain
{
    public readonly struct BattleResult
    {
        public int WinnerArmyIndex { get; }
        public float ElapsedTime { get; }
        public int Survivors { get; }

        public BattleResult(int winnerArmyIndex, float elapsedTime, int survivors)
        {
            WinnerArmyIndex = winnerArmyIndex;
            ElapsedTime = elapsedTime;
            Survivors = survivors;
        }
    }
}
