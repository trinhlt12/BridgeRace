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

            if (this._bot != null)
            {
                this._bot.GetComponent<NavMeshAgent>().ResetPath();
            }
        }
    }
}