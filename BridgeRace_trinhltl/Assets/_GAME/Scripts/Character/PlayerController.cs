namespace _GAME.Scripts.Character
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class PlayerController : Character
    {
        [SerializeField] private FloatingJoystick joystick;
        public                   Rigidbody        rb;

        protected override void OnInit()
        {
            base.OnInit();
            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
            }
        }

        private Vector3 _lastPosition;
        private bool    _isMovingDownBridge = false;

        private void Update()
        {
            if (IsOnBridge)
            {
                _isMovingDownBridge = transform.position.y < _lastPosition.y;
                Debug.Log($"Is Moving Down Bridge: {_isMovingDownBridge}");
            }

            _lastPosition = transform.position;
        }

        protected override void InitializeStates()
        {
            if (this._stateMachine != null)
            {
                this._stateMachine.AddState(new PlayerIdleState(this._stateMachine, character: this));
                this._stateMachine.AddState(new PlayerMovingState(this._stateMachine, character: this));
            }
            else
            {
                Debug.LogError("State machine is null in PlayerController.InitializeStates");
            }
        }

        public Vector2 GetMovementInput()
        {
            if (joystick == null)
            {
                Debug.LogWarning("Joystick is null in PlayerController.GetMovementInput");
                return Vector2.zero;
            }

            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }

        private bool IsMovingDownBridge()
        {
            if (!IsOnBridge) return false;

            var velocity = rb.velocity;

            return velocity.y < -0.1f;
        }

    }
}