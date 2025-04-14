namespace _GAME.Scripts.FSM.BotStates
{
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
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
        private Brick _targetBrick;
        private Floor _currentFloor => FloorManager.Instance.GetCurrentFloorObject();
        private FloorGate _currentFloorGate => _currentFloor.GetComponent<Floor>().floorGate;

        public BotMoveState(StateMachine stateMachine, Character character) :
            base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();

            BrickSpawner.Instance.OnBricksSpawned += HandleBricksSpawned;

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
            float minDistance  = float.MaxValue;

            if (BrickSpawner.Instance != null &&
                BrickSpawner.Instance._activeBricks.TryGetValue(this._bot.characterColor, out var bricks))
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

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (BrickSpawner.Instance != null &&
                (!BrickSpawner.Instance._activeBricks.TryGetValue(this._bot.characterColor, out var bricks) || bricks.Count == 0))
            {
                _bot.SetDestination(this._currentFloorGate.transform.position);
            }

            this._targetBrick = FindNearestBrick();
            if (this._targetBrick != null)
            {
                this._targetPosition = this._targetBrick.transform.position;
                this._bot.SetDestination(this._targetPosition);
            }

            if (this._bot.HasReachedDestination())
            {
                _targetBrick = null;
            }
        }


        public override void OnExit()
        {
            base.OnExit();
            BrickSpawner.Instance.OnBricksSpawned -= HandleBricksSpawned;
        }
    }
}