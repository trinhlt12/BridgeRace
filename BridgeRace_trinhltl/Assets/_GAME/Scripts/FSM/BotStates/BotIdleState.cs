namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using UnityEngine.AI;

    public class BotIdleState : BaseState
    {
        public BotIdleState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();

            this._bot.ResetDestination();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (this._elapsedTime >= 0.25f)
            {
                this._stateMachine.ChangeState<FindBrickState>();
                return;
            }
        }
    }
}