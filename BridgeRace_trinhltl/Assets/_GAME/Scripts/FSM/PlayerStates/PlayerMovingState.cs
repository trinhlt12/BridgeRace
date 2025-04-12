namespace _GAME.Scripts.FSM.PlayerStates
{
    using UnityEngine;

    public class PlayerMovingState : BaseState
    {
        private Vector2 _currentInput;

        public PlayerMovingState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
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

            if (this._currentInput.magnitude > 0.1f)
            {
                this.Move();
            }
        }

        private void Move()
        {
            var moveDirection =
                new Vector3(this._currentInput.x, 0, this._currentInput.y).normalized;

            this._player.rb.velocity = moveDirection * this._player.moveSpeed;

            RaycastHit hit;
            var surfaceDetected = Physics.Raycast(
                this._player.transform.position + Vector3.up * 0.1f,
                Vector3.down,
                out hit,
                1.0f
            );

            Debug.DrawRay(
                this._player.transform.position + Vector3.up * 0.1f,
                Vector3.down,
                Color.red
            );

            if (surfaceDetected)
            {
                var surfaceNormal = hit.normal;

                var slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

                if (slopeAngle < 45f)
                {
                    moveDirection = Vector3.ProjectOnPlane(moveDirection, surfaceNormal).normalized;
                    var slopeGravity = Vector3.ProjectOnPlane(
                        Physics.gravity,
                        surfaceNormal
                    );

                    this._player.rb.AddForce(-slopeGravity, ForceMode.Acceleration);
                    this._player.rb.drag = 5;
                }
                else
                {
                    this._player.rb.drag = 0.5f;
                }

                this._player.rb.velocity = new Vector3(
                    moveDirection.x * this._player.moveSpeed,
                    this._player.rb.velocity.y,
                    moveDirection.z * this._player.moveSpeed
                );
            }
            else
            {
                this._player.rb.drag = 0;
                this._player.rb.velocity = new Vector3(
                    moveDirection.x * this._player.moveSpeed,
                    this._player.rb.velocity.y,
                    moveDirection.z * this._player.moveSpeed
                );
            }

            /*this._player.rb.velocity = moveDirection * this._player.moveSpeed;*/

            if (moveDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(moveDirection,
                    surfaceDetected ? hit.normal : Vector3.up);
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
            this._currentInput = Vector2.zero;
        }
    }
}