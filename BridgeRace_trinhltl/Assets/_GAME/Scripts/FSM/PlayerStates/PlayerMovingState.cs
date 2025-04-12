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
            if(this._player == null || this._player.rb == null)
            {
                return;
            }

            if (this._currentInput.magnitude > 0.1f)
            {
                var moveDirection =
                    new Vector3(this._currentInput.x, 0, this._currentInput.y).normalized;
                this._player.rb.velocity = moveDirection * this._player.moveSpeed;

                if (moveDirection != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(moveDirection);
                    this._player.rb.rotation = Quaternion.Slerp(
                        this._player.rb.rotation,
                        targetRotation,
                        this._player.rotationSpeed * Time.fixedDeltaTime
                    );
                }
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