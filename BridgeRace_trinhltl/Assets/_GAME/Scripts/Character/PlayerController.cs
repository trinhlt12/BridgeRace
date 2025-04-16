namespace _GAME.Scripts.Character
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.Bridge;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class PlayerController : Character
    {
        [SerializeField] private FloatingJoystick    joystick;
        public                   LayerMask           bridgeLayerMask;
        public                   Vector3             _lastPosition;
        public                   CharacterController characterController;
        public                   Vector3             velocity;

        protected override void OnInit()
        {
            base.OnInit();
            if (this.characterController == null)
            {
                this.characterController = this.GetComponent<CharacterController>();
            }

            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
            }
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
                return Vector2.zero;
            }

            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }

        public bool IsMovingDownTheBridge()
        {
            /*var playerVelocity = this.transform.position - this._lastPosition;*/
            var inputDirection = this.GetMoveDirection();
            if (inputDirection.magnitude < 0.1f) return false;

            var rayStart     = this.transform.position + Vector3.up * 0.1f;
            var rayDirection = Vector3.down;
            var rayDistance  = 1.5f;

            var dotProduct = Vector3.Dot(inputDirection, this._currentBridgeForward);

            if (dotProduct < -0.3f)
            {
                return true;
            }

            if (dotProduct <= 0)
            {
                Debug.Log("Moving down the bridge");
                return true;
            }
            else if (dotProduct > 0)
            {
                Debug.Log("Moving up the bridge");
                return false;
            }

            return false;
        }

        private Vector3 GetMoveDirection()
        {
            return new Vector3(this.GetMovementInput().x, 0, this.GetMovementInput().y).normalized;
        }

        public bool CanMove()
        {
            if (!IsOnBridge || this.IsMovingDownTheBridge()) return true;

            if (this._currentBridgeStep == null) return true;

            var isColorMatch = _currentBridgeStep.IsColorMatch(this.characterColor);

            if (!isColorMatch)
            {
                if (BrickCount <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}