namespace SimpleArmyClash.Application
{
    public sealed class PreparingGameState : IGameState
    {
        private readonly BattlePreparationService _preparation;
        private readonly BattlePresentation _presentation;
        private readonly BattleReadModel _readModel;

        public GamePhaseType Phase => GamePhaseType.Preparing;

        public PreparingGameState(BattlePreparationService preparation, BattlePresentation presentation, BattleReadModel readModel)
        {
            _preparation = preparation;
            _presentation = presentation;
            _readModel = readModel;
        }

        public void Enter()
        {
            _preparation.Generate();
            _presentation.Show(_preparation.Units);
            _readModel.SetCounts(_preparation.Units.Length / 2, _preparation.Units.Length / 2);
            _readModel.SetElapsedTime(0f);
        }

        public void Exit()
        {
            _readModel.SelectUnit(-1);
            _presentation.Clear();
        }
    }
}
