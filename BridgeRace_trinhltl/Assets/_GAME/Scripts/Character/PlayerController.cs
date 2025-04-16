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
        [SerializeField] private FloatingJoystick joystick;
        public                   LayerMask        bridgeLayerMask;
        public                   Rigidbody        rb;
        public                  Vector3          _lastPosition;

        protected override void OnInit()
        {
            base.OnInit();

            if (this.rb == null)
            {
                this.rb = this.GetComponent<Rigidbody>();
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

        /*public bool CanMoveForward()
        {
            var forwardPosition = this.transform.position + this.transform.forward * 0.5f + Vector3.up * 0.1f;

            var rayDirection = new Vector3(this.transform.forward.x, -0.5f, this.transform.forward.z).normalized;
            var rayDistance  = 1.5f;

            Debug.DrawRay(forwardPosition, rayDirection * rayDistance, Color.red);

            if (Physics.Raycast(forwardPosition, rayDirection, out var hit, rayDistance, bridgeLayerMask))
            {
                var bridgeStep = hit.collider.GetComponent<BridgeStep>();
                Debug.LogWarning(hit.collider.name);
                if (bridgeStep != null)
                {
                    if (bridgeStep.IsColorMatch(this.characterColor))
                    {
                        return true;
                    }
                    else
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
                }
            }

            return true;
        }*/

        public bool IsMovingDownTheBridge()
        {
            var playerVelocity = this.transform.position - this._lastPosition;
            if (playerVelocity.magnitude <= 0.1f) return false;
            var rayStart      = this.transform.position + Vector3.up * 0.1f;
            var rayDirection  = Vector3.down;
            var rayDistance   = 1.5f;

            if (Physics.Raycast(rayStart, rayDirection, out var hit, rayDistance, this.bridgeLayerMask))
            {
                var dotProduct    = Vector3.Dot(playerVelocity, hit.transform.forward);

                Debug.DrawRay(hit.point, hit.transform.forward * 2f, Color.blue, 0.1f);
                Debug.DrawRay(hit.point, playerVelocity * 2f, Color.red, 0.1f);

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