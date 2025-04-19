namespace _GAME.Scripts.FSM.BotStates
{
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class BotBaseState : BaseState
    {

        private   Transform _targetTransform;
        protected Vector3   _targetPosition;

        protected        Brick           _targetBrick;
        protected static Floor           _currentFloor     => FloorManager.Instance.GetCurrentFloorObject();
        protected static List<FloorGate> _currentFloorGate => _currentFloor.GetComponent<Floor>().floorGate;

        public BotBaseState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();

        }

        public void RecalculateTarget()
        {
            this._targetBrick                = null;
            this._bot.currentTargetGateIndex = -1;
        }
    }
}