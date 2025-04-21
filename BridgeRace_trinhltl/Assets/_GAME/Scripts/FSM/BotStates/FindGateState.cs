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
            var currentFloor = FloorManager.Instance.GetCurrentFloorForCharacter(this._bot);
            Debug.Log($"Bot {_bot.name} is on floor: {currentFloor?.name}");

            if (currentFloor == null) return;

            if (_bot.currentTargetGateIndex >= 0 && _bot.currentTargetFloor != null &&
                GateTargetManager.Instance.IsGateReservedForBot(_bot.currentTargetFloor, _bot.currentTargetGateIndex, _bot))
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0 && _bot.currentTargetFloor != null)
            {
                GateTargetManager.Instance.ReleaseGate(_bot.currentTargetFloor, _bot.currentTargetGateIndex, _bot);
            }

            var newTargetGateIndex = GateTargetManager.Instance.NearestAvailableGate(this._bot, currentFloor);

            if (newTargetGateIndex >= 0)
            {
                if (GateTargetManager.Instance.ReserveGate(currentFloor, newTargetGateIndex, _bot))
                {
                    _bot.currentTargetFloor     = currentFloor;
                    _bot.currentTargetGateIndex = newTargetGateIndex;

                    if (currentFloor.floorGate != null && newTargetGateIndex < currentFloor.floorGate.Count)
                    {
                        _targetPosition = currentFloor.floorGate[newTargetGateIndex].transform.position;
                        _bot.SetDestination(_targetPosition);
                    }
                }
            }
            else
            {
                _bot.currentTargetGateIndex = -1;
                _bot.currentTargetFloor     = null;
            }
        }
    }
}