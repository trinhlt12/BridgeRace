namespace _GAME.Scripts.FSM.PlayerStates
{
    using UnityEngine;

    public class PlayerIdleState : BaseState
    {
        public PlayerIdleState(StateMachine stateMachine, Character character) :
            base(stateMachine, character) { }
        public override void OnEnter()
        {
            base.OnEnter();
            this._character.rb.velocity = Vector3.zero;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}