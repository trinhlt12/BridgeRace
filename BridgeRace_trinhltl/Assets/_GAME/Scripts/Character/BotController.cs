namespace _GAME.Scripts.Character
{
    using System.Collections;
    using _GAME.Scripts.Floor;
    using _GAME.Scripts.FSM.BotStates;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.Bridge;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Serialization;

    public class BotController : Character
    {
        public                  NavMeshAgent navMeshAgent;
        [SerializeField] public float        _minDistanceToTarget   = 0.5f;
        public                  bool         IgnoreCondition        = false;

        public                  Floor        currentTargetFloor;
        public                  int          currentTargetGateIndex = -1;

        protected override void OnInit()
        {
            base.OnInit();

            navMeshAgent = GetComponent<NavMeshAgent>();

            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<FindBrickState>();
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
                this._stateMachine.AddState(new FindBrickState(this._stateMachine, character: this));
                this._stateMachine.AddState(new FindGateState(this._stateMachine, character: this));
            }
        }

        public void StopAgent(bool stop)
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                this.navMeshAgent.isStopped = stop;
                this.navMeshAgent.updatePosition = !stop;
                this.navMeshAgent.updateRotation = !stop;
            }
        }

        public void SetDestination(Vector3 destination)
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                StopAgent(false);
                this.navMeshAgent.SetDestination(destination);
            }
        }

        public void ResetDestination()
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                this.navMeshAgent.enabled = false;
                if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
                {
                    this.navMeshAgent.ResetPath();
                }
                this.navMeshAgent.enabled = true;
            }
        }

        public bool HasReachedDestination()
        {
            if (this.navMeshAgent != null && this.navMeshAgent.isActiveAndEnabled)
            {
                return !this.navMeshAgent.pathPending
                    && this.navMeshAgent.remainingDistance
                    <= this.navMeshAgent.stoppingDistance;
            }
            return false;
        }

        public bool IsLevelCompleted()
        {
            var currentFloor = FloorManager.Instance.GetCurrentFloorIndexForCharacter(this);
            if (currentFloor >= FloorManager.Instance.highestFloorIndex)
            {
                return true;
            }else
            {
                return false;
            }
        }
    }
}