namespace _GAME.Scripts.FSM
{
    using _GAME.Scripts.Player;

    public class BaseState : IState
    {
        protected StateMachine     _stateMachine;
        protected Character        _character;
        protected PlayerController _player;

        public BaseState(StateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            this._player = character as PlayerController;
        }

        public virtual void OnEnter()
        {
            PlayAnimation(this);
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        private void PlayAnimation(IState state)
        {
            if (this._player.animator == null) return;

            this._player.animator.Play(state.GetType().Name);
        }
    }
}