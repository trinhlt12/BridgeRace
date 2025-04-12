namespace _GAME.Scripts.FSM
{
    public class BaseState : IState
    {
        protected StateMachine _stateMachine;
        protected Character    _character;

        public BaseState(StateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character    = character;
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
            if (_character.animator == null) return;

            _character.animator.Play(state.GetType().Name);
        }
    }
}