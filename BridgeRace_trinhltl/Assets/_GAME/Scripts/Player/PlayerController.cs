namespace _GAME.Scripts.Player
{
    using System;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class PlayerController : Character
    {
        [SerializeField] private FloatingJoystick joystick;
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnInit()
        {
            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
            }
            else
            {
                Debug.LogError("State machine is null in PlayerController.OnInit");
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
                Debug.LogWarning("Joystick is null in PlayerController.GetMovementInput");
                return Vector2.zero;
            }

            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }
    }
}