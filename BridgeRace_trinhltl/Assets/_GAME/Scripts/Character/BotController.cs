namespace _GAME.Scripts.Character
{
    using _GAME.Scripts.FSM.BotStates;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;
    using UnityEngine.AI;

    public class BotController : Character
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float        _minDistanceToTarget = 0.5f;

        protected override void OnInit()
        {
            base.OnInit();

            /*var settings = NavMesh.GetSettingsByID(0);

            settings.agentSlope = 60f;
            settings.agentClimb = 0.5f;*/

            this.navMeshAgent.speed        = this.moveSpeed;
            this.navMeshAgent.angularSpeed = this.rotationSpeed * 10f;

            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<BotMoveState>();
            }
            else
            {
                Debug.LogError("State machine is null in BotController.OnInit");
            }
        }

        protected override void InitializeStates()
        {
            if (this._stateMachine != null)
            {
                this._stateMachine.AddState(new BotIdleState(this._stateMachine, character: this));
                this._stateMachine.AddState(new BotMoveState(this._stateMachine, character: this));
            }
        }

        public void SetDestination(Vector3 destination)
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                this.navMeshAgent.SetDestination(destination);
            }
        }

        public bool HasReachedDestination()
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                return !this.navMeshAgent.pathPending
                    && this.navMeshAgent.remainingDistance
                    <= this._minDistanceToTarget;
            }
            return false;
        }
    }
}