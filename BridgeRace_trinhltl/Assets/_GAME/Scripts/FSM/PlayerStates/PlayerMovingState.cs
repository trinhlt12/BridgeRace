using _GAME.Scripts.Character;
using UnityEngine;

namespace _GAME.Scripts.FSM.PlayerStates
{
    using Character = _GAME.Scripts.Character.Character;

    public class PlayerMovingState : BaseState
    {
        private Vector2 _currentInput;
        private Vector3 _groundNormal        = Vector3.up;
        private bool    _isGrounded          = true;
        private float   _groundCheckDistance = 0.2f;
        private float   _slopeLimit          = 45f;

        private readonly float   _gravity          = -9.81f;
        private          float   _verticalVelocity = 0;
        private          float   _acceleration     = 15f;
        private          float   _deceleration     = 20f;
        private          Vector3 _currentVelocity  = Vector3.zero;
        private          float   _maxAllowedSpeed  = 8f;

        private Vector3 _originalPosition;
        private Vector3 _lastPosition;

        public PlayerMovingState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
            _originalPosition = this._player.transform.position;
            _currentVelocity  = Vector3.zero;
            _verticalVelocity = 0;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();


            this._currentInput = this._player.GetMovementInput();

            if (this._currentInput.magnitude < 0.1f)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
                return;
            }

            Move(Time.deltaTime);
            this._player._lastPosition = this._player.transform.position;
        }

        private void Move(float deltaTime)
        {
            if (!this._player.CanMove()  || !this._player.IsMovingDownTheBridge())
            {
                return;
            }

            _lastPosition = this._player.transform.position;

            var moveDirection = new Vector3(this._currentInput.x, 0, this._currentInput.y).normalized;

            _isGrounded = this._player.characterController.isGrounded;

            if (_isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += _gravity * deltaTime;
            }

            var targetVelocity = moveDirection * this._player.moveSpeed;

            if (moveDirection.magnitude > 0.1f)
            {
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    targetVelocity,
                    _acceleration * deltaTime
                );
            }
            else
            {
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    Vector3.zero,
                    _deceleration * deltaTime
                );
            }

            var horizontalSpeed = new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
            if (horizontalSpeed > _maxAllowedSpeed)
            {
                var limitFactor = _maxAllowedSpeed / horizontalSpeed;
                _currentVelocity = new Vector3(
                    _currentVelocity.x * limitFactor,
                    _currentVelocity.y,
                    _currentVelocity.z * limitFactor
                );
            }

            var finalVelocity = _currentVelocity + new Vector3(0, _verticalVelocity, 0);

            this._player.characterController.Move(finalVelocity * deltaTime);

            this._player.velocity = finalVelocity;

            RotateTowardsMoveDirection();
        }

        private void RotateTowardsMoveDirection()
        {
            var movementDirection = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);

            if (movementDirection.magnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(movementDirection.normalized);

                this._player.transform.rotation = Quaternion.Slerp(
                    this._player.transform.rotation,
                    targetRotation,
                    this._player.rotationSpeed * Time.deltaTime
                );
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            this._currentInput = Vector2.zero;
        }
    }
}