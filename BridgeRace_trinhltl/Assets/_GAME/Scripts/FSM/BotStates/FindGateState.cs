namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
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

            if (_bot.currentTargetGateIndex < 0 || this._bot.currentTargetFloor == null)
            {
                FindAndSetTarget();
            }

            if ( _bot.currentTargetGateIndex >= 0 && this._bot.currentTargetFloor != null)
            {

                var gates = this._bot.currentTargetFloor.floorGate;
                if (gates != null && this._bot.currentTargetGateIndex < gates.Count)
                {
                    var gate = gates[this._bot.currentTargetGateIndex].transform.position;
                    var gateForward = gates[this._bot.currentTargetGateIndex].transform.forward;
                    var destination      = gate + gateForward * 0.5f;
                    _bot.SetDestination(destination);
                }
            }

            if (this._bot.HasReachedDestination())
            {
                this._stateMachine.ChangeState<FindBrickState>();
                return;
            }
        }

        private void FindAndSetTarget()
        {
            if (_currentFloor == null || _currentFloorGate == null || _currentFloorGate.Count == 0)
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0 && GateTargetManager.Instance.IsGateReservedForBot(this._bot.currentTargetFloor,_bot.currentTargetGateIndex, _bot))
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReleaseGate(this._bot.currentTargetFloor, _bot.currentTargetGateIndex, _bot);
            }

            var newTargetGateIndex = GateTargetManager.Instance.NearestAvailableGate(this._bot, _currentFloor);

            if (newTargetGateIndex >= 0 && !GateTargetManager.Instance.IsGateReservedForBot(this._bot.currentTargetFloor, newTargetGateIndex, _bot))
            {
                GateTargetManager.Instance.ReserveGate(_currentFloor, newTargetGateIndex, _bot);
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