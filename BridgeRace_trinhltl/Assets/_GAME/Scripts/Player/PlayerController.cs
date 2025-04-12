namespace _GAME.Scripts.Player
{
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class PlayerController : Character
    {
        [SerializeField] private FloatingJoystick joystick;

        protected override void Awake()
        {
            base.Awake();

            InitializeStates();

            this.OnInit();

        }

        protected override void OnInit()
        {
            this._stateMachine.ChangeState<PlayerIdleState>();
        }

        protected override void InitializeStates()
        {
            this._stateMachine.AddState(new PlayerIdleState(this._stateMachine, character: this));
            this._stateMachine.AddState(new PlayerMovingState(this._stateMachine, character: this));
        }

        public Vector2 GetMovementInput()
        {
            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }
    }
}