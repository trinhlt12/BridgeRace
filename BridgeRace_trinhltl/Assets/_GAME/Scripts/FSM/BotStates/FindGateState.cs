namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM.Bridge;
    using UnityEngine;

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
                var offset = Vector3.forward * 0.5f;
                _bot.SetDestination(destination + offset);
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

            var newTargetGateIndex = GateTargetManager.Instance.NearestAvailableGate(this._bot, _currentFloorGate);

            if (newTargetGateIndex >= 0 && !GateTargetManager.Instance.IsGateReservedForBot(newTargetGateIndex, _bot))
            {
                GateTargetManager.Instance.ReserveGate(newTargetGateIndex, _bot);
                this._bot.currentTargetGateIndex = newTargetGateIndex;
                this._targetPosition = _currentFloorGate[newTargetGateIndex].transform.position;
                this._bot.SetDestination(this._targetPosition);
            }else
            {
                this._bot.currentTargetGateIndex = -1;
            }
        }
    }
}