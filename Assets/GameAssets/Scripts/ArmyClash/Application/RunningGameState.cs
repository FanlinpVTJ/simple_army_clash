namespace SimpleArmyClash.Application
{
    public sealed class RunningGameState : IGameState
    {
        private readonly BattlePreparationService _preparation;
        private readonly BattleSessionService _session;
        private readonly BattlePresentation _presentation;
        private readonly BattleReadModel _readModel;

        public GamePhaseType Phase => GamePhaseType.Running;

        public RunningGameState(BattlePreparationService preparation, BattleSessionService session,
            BattlePresentation presentation, BattleReadModel readModel)
        {
            _preparation = preparation;
            _session = session;
            _presentation = presentation;
            _readModel = readModel;
        }

        public void Enter()
        {
            _session.Start(_preparation.CreateBattleUnits());
            _presentation.Show(_session.Units);
            _presentation.Attach(_session.Simulation);
            _readModel.SetCounts(_session.Simulation.GetAliveCount(0), _session.Simulation.GetAliveCount(1));
            _readModel.SetElapsedTime(0f);
        }

        public void Exit()
        {
            _presentation.Clear();
            _session.Stop();
        }
    }
}
