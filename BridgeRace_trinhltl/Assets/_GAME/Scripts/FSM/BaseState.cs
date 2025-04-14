namespace _GAME.Scripts.FSM
{
    using _GAME.Scripts.Character;
    using UnityEngine;

    public class BaseState : IState
    {
        protected StateMachine     _stateMachine;
        protected Character        _character;
        protected PlayerController _player;

        public BaseState(StateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character    = character;
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
            if (this._player.animator != null)
            {
                var stateName = state.GetType().Name;
                this._player.animator.Play(stateName);
                Debug.Log($"Attempting to play animation for state: {stateName}");
            }else
            {
                Debug.LogError("Animator is null, cannot play animation.");
            }
        }
    }
}