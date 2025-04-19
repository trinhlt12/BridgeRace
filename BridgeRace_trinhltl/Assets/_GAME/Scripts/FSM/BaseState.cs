namespace _GAME.Scripts.FSM
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM.BotStates;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class BaseState : IState
    {
        protected StateMachine     _stateMachine;
        protected Character        _character;
        protected PlayerController _player;
        protected BotController _bot;
        protected float _elapsedTime = 0f;

        public BaseState(StateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character    = character;
            this._player = character as PlayerController;
            this._bot = character as BotController;
        }

        public virtual void OnEnter()
        {
            PlayAnimation(this);
        }

        public virtual void OnUpdate()
        {
            this._elapsedTime += Time.deltaTime;
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnExit()
        {
            this._elapsedTime = 0f;
        }

        private void PlayAnimation(IState state)
        {
            Animator animator = null;

            if (IsPlayer() && _player.animator != null)
            {
                animator = _player.animator;
            }
            else if (IsBot() && _bot.animator != null)
            {
                animator = _bot.animator;
            }

            if (animator != null)
            {
                var animationName = GetAnimationNameForState(state);
                animator.Play(animationName);
            }
        }

        private static string GetAnimationNameForState(IState state)
        {
            var stateType = state.GetType();

            // Player animations
            if (stateType == typeof(PlayerIdleState))
                return "Idle";
            if (stateType == typeof(PlayerMovingState))
                return "Move";

            // Bot animations
            if (stateType == typeof(BotIdleState))
                return "Idle";
            if (stateType == typeof(FindGateState) || stateType == typeof(FindBrickState))
                return "Move";

            Debug.LogWarning($"No animation mapping found for state {stateType.Name}, using state name as fallback.");
            return stateType.Name;
        }

        protected bool IsPlayer()
        {
            return _player != null;
        }

        protected bool IsBot()
        {
            return _bot != null;
        }
    }
}