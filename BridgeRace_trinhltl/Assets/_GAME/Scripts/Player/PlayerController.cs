namespace _GAME.Scripts.Player
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.PlayerStates;
    using UnityEngine;

    public class PlayerController : Character
    {
        [SerializeField] private FloatingJoystick joystick;
        [SerializeField] private Transform        BrickHolder;
        public                   float            moveSpeed     = 5f;
        public                   float            rotationSpeed = 10f;

        private List<Brick> _pickedBricks = new List<Brick>();

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnInit()
        {
            SetCharacterColor(characterColor);

            if (this._stateMachine != null)
            {
                this._stateMachine.ChangeState<PlayerIdleState>();
            }
        }

        protected override void InitializeStates()
        {
            if (this._stateMachine != null)
            {
                this._stateMachine.AddState(new PlayerIdleState(this._stateMachine, character: this));
                this._stateMachine.AddState(new PlayerMovingState(this._stateMachine, character: this));
            }
            else
            {
                Debug.LogError("State machine is null in PlayerController.InitializeStates");
            }
        }

        public Vector2 GetMovementInput()
        {
            if (joystick == null)
            {
                Debug.LogWarning("Joystick is null in PlayerController.GetMovementInput");
                return Vector2.zero;
            }

            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }

        public override void pickUpBrick(Brick brick)
        {
            base.pickUpBrick(brick);
            _pickedBricks.Add(brick);

            brick.transform.SetParent(BrickHolder);
            brick.transform.localPosition = Vector3.zero;
            brick.transform.localRotation = Quaternion.identity;

            var brickCollider = brick.GetComponent<BoxCollider>();
            var brickHeight  = brickCollider.bounds.size.y;

            brick.gameObject.GetComponent<Collider>().enabled = false;

            var brickVisual = brick.transform.GetChild(0);

            var vector3     = brickVisual.position;
            vector3.y            = this._pickedBricks.Count * brickHeight;
            brickVisual.position = vector3;
        }

        public override void placeBrick(Brick brick)
        {
            base.placeBrick(brick);
        }

        public Brick GetTopBrick()
        {
            if (this.brickStack.Count > 0)
            {
                var topBrick = brickStack.Peek();
                return topBrick;
            }
            else
            {
                return null;
            }
        }
    }
}