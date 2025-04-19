namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class FindBrickState : BotBaseState
    {
        public FindBrickState(StateMachine stateMachine, Character character)
            : base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();
            this._bot.ResetDestination();
            BrickSpawner.Instance.OnBricksSpawned += HandleBricksSpawned;

        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            this.MoveToNearestBrick();
            if (this._bot.BrickCount >= 5)
            {
                this._stateMachine.ChangeState<FindGateState>();
                return;
            }
        }

        private void HandleBricksSpawned(BrickColor color, int count)
        {
            if (color == this._bot.characterColor && this._targetBrick == null)
            {
                _targetBrick = FindNearestBrick();
            }
        }

        private Brick FindNearestBrick()
        {
            Brick nearestBrick = null;
            var   minDistance  = float.MaxValue;

            if (BrickSpawner.Instance != null && BrickSpawner.Instance._activeBricks.TryGetValue(this._bot.characterColor, out var bricks))
            {
                foreach (var brick in bricks)
                {
                    if (brick != null && brick.gameObject.activeInHierarchy)
                    {
                        var distance = Vector3.Distance(this._bot.transform.position, brick.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance  = distance;
                            nearestBrick = brick;
                        }
                    }
                }
            }
            return nearestBrick;
        }

        private void MoveToNearestBrick()
        {
            if (BrickSpawner.Instance._activeBricks.TryGetValue(_bot.characterColor, out var bricks) && bricks.Count > 0)
            {
                _targetBrick = FindNearestBrick();
                if (_targetBrick != null)
                {
                    _targetPosition = _targetBrick.transform.position;
                    _bot.SetDestination(_targetPosition);
                }

                if (this._bot.HasReachedDestination())
                {
                    _targetBrick = null;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            BrickSpawner.Instance.OnBricksSpawned -= HandleBricksSpawned;

        }
    }
}