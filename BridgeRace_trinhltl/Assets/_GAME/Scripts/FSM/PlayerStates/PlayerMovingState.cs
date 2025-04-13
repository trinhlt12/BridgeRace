namespace _GAME.Scripts.FSM.PlayerStates
{
    using UnityEngine;

    public class PlayerMovingState : BaseState
    {
        private Vector2 _currentInput;
        private Vector3 _groundNormal = Vector3.up;
        private bool _isGrounded = true;
        private float _groundCheckDistance = 0.2f;
        private float _slopeLimit = 45f;
        private float _groundedGravity = -1f;
        private float _maxAllowedSpeed = 8f;

        private float _acceleration = 15f;
        private float _deceleration = 20f;
        private Vector3 _currentVelocity = Vector3.zero;
        private float _airControl = 0.7f;

        public PlayerMovingState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
            if (this._player != null && this._player.rb != null)
            {
                _currentVelocity = this._player.rb.velocity;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            this._currentInput = this._player.GetMovementInput();
            if (this._currentInput.magnitude < 0.1f)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (this._player == null || this._player.rb == null)
            {
                return;
            }

            UpdateGroundState();
            Move(Time.fixedDeltaTime);
            RotateTowardsMoveDirection();
        }

        private void UpdateGroundState()
        {
            RaycastHit hit;
            Vector3 rayStart = this._player.transform.position + Vector3.up * 0.1f;

            _isGrounded = Physics.Raycast(
                rayStart,
                Vector3.down,
                out hit,
                _groundCheckDistance + 0.1f
            );

            Debug.DrawRay(
                rayStart,
                Vector3.down * (_groundCheckDistance + 0.1f),
                _isGrounded ? Color.green : Color.red
            );

            if (_isGrounded)
            {
                _groundNormal = hit.normal;

                float slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);

                if (slopeAngle > _slopeLimit)
                {
                    _isGrounded = false;
                    _groundNormal = Vector3.up;
                }
            }
            else
            {
                _groundNormal = Vector3.up;
            }
        }

        private void Move(float deltaTime)
        {
            Vector3 moveDirection = new Vector3(this._currentInput.x, 0, this._currentInput.y).normalized;

            if (_isGrounded)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, _groundNormal).normalized;
            }

            Vector3 targetVelocity = moveDirection * this._player.moveSpeed;

            if (_isGrounded)
            {
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
                Vector3 gravity = _groundNormal * _groundedGravity;
                _currentVelocity += gravity * deltaTime;
            }
            else
            {
                Vector3 horizontalVel = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);
                horizontalVel = Vector3.Lerp(
                    horizontalVel,
                    targetVelocity * _airControl,
                    _acceleration * _airControl * deltaTime
                );

                _currentVelocity = new Vector3(horizontalVel.x, this._player.rb.velocity.y, horizontalVel.z);
            }

            float horizontalSpeed = new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
            if (horizontalSpeed > _maxAllowedSpeed)
            {
                float limitFactor = _maxAllowedSpeed / horizontalSpeed;
                _currentVelocity = new Vector3(
                    _currentVelocity.x * limitFactor,
                    _currentVelocity.y,
                    _currentVelocity.z * limitFactor
                );
            }
            this._player.rb.velocity = _currentVelocity;
        }

        private void RotateTowardsMoveDirection()
        {
            Vector3 movementDirection = new Vector3(this._player.rb.velocity.x, 0, this._player.rb.velocity.z);

            if (movementDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection.normalized, _groundNormal);

                this._player.rb.rotation = Quaternion.Slerp(
                    this._player.rb.rotation,
                    targetRotation,
                    this._player.rotationSpeed * Time.fixedDeltaTime
                );
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this._player == null || this._player.rb == null)
            {
                return;
            }

            Vector3 currentVel = this._player.rb.velocity;
            this._player.rb.velocity = new Vector3(currentVel.x * 0.5f, currentVel.y, currentVel.z * 0.5f);

            this._currentInput = Vector2.zero;
        }
    }
}