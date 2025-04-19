namespace _GAME.Scripts.FSM.BotStates
{
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.Bridge;
    using UnityEngine;

    public enum BotTargetType
    {
        Brick,
        Bridge,
        Idle
    }

    public class BotMoveState : BaseState
    {
        private Transform     _targetTransform;
        private Vector3       _targetPosition;

        private        Brick           _targetBrick;
        private static Floor           _currentFloor     => FloorManager.Instance.GetCurrentFloorObject();
        private static List<FloorGate> _currentFloorGate => _currentFloor.GetComponent<Floor>().floorGate;

        public BotMoveState(StateMachine stateMachine, Character character) :
            base(stateMachine, character) { }

        public override void OnEnter()
        {
            base.OnEnter();

            BrickSpawner.Instance.OnBricksSpawned += HandleBricksSpawned;

            FindAndSetTarget();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (this._bot.IsOnBridge && !this._bot.CanMove())
            {
                this._bot.StopAgent(true);
            }

            if (_bot.brickStack.Count >= 1 && _bot.currentTargetGateIndex >= 0 && this._bot.CanMove())
            {
                _bot.SetDestination(_currentFloorGate[_bot.currentTargetGateIndex].transform.position);
                return;
            }

            if (_bot.brickStack.Count >= 1 && _bot.currentTargetGateIndex < 0)
            {
                FindAndSetTarget();
            }
            this.MoveToNearestBrick();
        }

        /*public void RecalculateTarget()
        {
            this._targetBrick                = null;
            this._bot.currentTargetGateIndex = -1;
        }*/

        private void FindAndSetTarget()
        {
            if (_currentFloor == null || _currentFloorGate == null || _currentFloorGate.Count == 0)
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0 && GateTargetManager.Instance.IsGateReservedForBot(_bot.currentTargetGateIndex, _bot))
            {
                return;
            }

            if (_bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReleaseGate(_bot.currentTargetGateIndex, _bot);
            }

            this._bot.currentTargetGateIndex = GateTargetManager.Instance.NearestAvailableGate(this._bot, _currentFloorGate);

            if (this._bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReserveGate(_bot.currentTargetGateIndex, _bot);
                this._targetPosition = _currentFloorGate[this._bot.currentTargetGateIndex].transform.position;
                this._bot.SetDestination(this._targetPosition);
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
            if (_bot.currentTargetGateIndex >= 0)
            {
                GateTargetManager.Instance.ReleaseGate(_bot.currentTargetGateIndex, _bot);
                _bot.currentTargetGateIndex = -1;
            }
            BrickSpawner.Instance.OnBricksSpawned -= HandleBricksSpawned;
        }
    }
}