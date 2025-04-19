namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM.Bridge;

    public class FindGateState : BotBaseState
    {
        public FindGateState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
            this._bot.ResetDestination();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!this._bot.CanMove())
            {
                this._stateMachine.ChangeState<FindBrickState>();
                return;
            }

            if (_bot.currentTargetGateIndex < 0)
            {
                FindAndSetTarget();
            }

            if ( _bot.currentTargetGateIndex >= 0)
            {
                var destination = _currentFloorGate[_bot.currentTargetGateIndex].transform.position;
                _bot.SetDestination(destination);
            }
        }

        private void FindAndSetTarget()
        {
            if (_currentFloor == null || _currentFloorGate == null || _currentFloorGate.Count == 0)
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0 && GateTargetManager.Instance.IsGateReservedForBot(_bot.currentTargetGateIndex, _bot))
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReleaseGate(_bot.currentTargetGateIndex, _bot);
            }

            this._bot.currentTargetGateIndex = GateTargetManager.Instance.NearestAvailableGate(this._bot, _currentFloorGate);

            if (this._bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReserveGate(_bot.currentTargetGateIndex, _bot);
                this._targetPosition = _currentFloorGate[this._bot.currentTargetGateIndex].transform.position;
                this._bot.SetDestination(this._targetPosition);
            }
        }
    }
}