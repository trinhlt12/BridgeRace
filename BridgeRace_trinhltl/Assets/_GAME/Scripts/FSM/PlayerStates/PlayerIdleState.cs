namespace _GAME.Scripts.FSM.PlayerStates
{
    using _GAME.Scripts.Character;
    using UnityEngine;

    public class PlayerIdleState : BaseState
    {
        public PlayerIdleState(StateMachine stateMachine, Character character) :
            base(stateMachine, character)
        {

        }
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            var input = this._player.GetMovementInput();
            if (input.magnitude > 0.1f)
            {
                this._stateMachine.ChangeState<PlayerMovingState>();
            }
        }
    }
}