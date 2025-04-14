namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public enum BotTargetType
    {
        Brick,
        Bridge,
        Idle
    }

    public class BotMoveState : BaseState
    {
        private BotTargetType currentTargetType = BotTargetType.Idle;
        private Transform     _targetTransform;
        private Vector3       _targetPosition;

        private float _lastTargetFindTime = 0f;
        private float _findTargetCooldown = 1f;

        public BotMoveState(StateMachine stateMachine, Character character) :
            base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
            this.FindNextTarget();
            Debug.Log(_targetTransform);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(this._bot == null) return;

            if (this._bot.HasReachedDestination())
            {
                HandleTargetReached();
            }

            if (this._elapsedTime >= this._lastTargetFindTime + this._findTargetCooldown)
            {
                this.FindNextTarget();
                _lastTargetFindTime = Time.time;
            }
        }

        private void HandleTargetReached()
        {
            throw new System.NotImplementedException();
        }

        private void FindNextTarget()
        {
            FindBrickTarget();

            /*if (this._bot.BrickCount > 0)
            {
                //find bridge target
                /*FindBridgeTarget();#1#
            }
            else
            {

            }*/
        }

        private void FindBridgeTarget()
        {
           //
        }

        private void FindBrickTarget()
        {
            var nearestBrick = this.FindNearestBrick();

            if(nearestBrick != null)
            {
                this.currentTargetType = BotTargetType.Brick;

                this._targetTransform = nearestBrick.transform;

                this._targetPosition = this._targetTransform.position;

                this._bot.SetDestination(this._targetPosition);
            }
            else
            {
                //Find Bridge target
                this._stateMachine.ChangeState<BotIdleState>();
            }
        }

        public Brick FindNearestBrick()
        {
            Brick nearestBrick = null;
            var   minDistance  = float.MaxValue;

            if (BrickSpawner.Instance._activeBricks.TryGetValue(this._bot.characterColor, out var brickList))
            {
                foreach (var brick in brickList)
                {
                    if (brick != null && brick.gameObject.activeInHierarchy)
                    {
                        var distance = Vector3.Distance(this._bot.transform.position,
                            brick.gameObject.transform.position);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestBrick = brick;
                        }
                    }
                }
            }
            return nearestBrick;
        }
    }
}