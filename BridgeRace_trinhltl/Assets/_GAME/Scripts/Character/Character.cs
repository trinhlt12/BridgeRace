namespace _GAME.Scripts.Character
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.FSM.Bridge;
    using UnityEngine;
    using UnityEngine.Serialization;

    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected StateMachine _stateMachine;
        [SerializeField] private   Transform    BrickHolder;
        public BridgeBlocker _bridgeBlocker;

        public BrickColor characterColor;
        public float      moveSpeed     = 5f;
        public float      rotationSpeed = 10f;

        public                                       Animator           animator;
        [SerializeField]                     private Renderer           _renderer;
        public                                       int                BrickCount => this.brickStack.Count;

        public readonly                             Stack<Brick> brickStack = new Stack<Brick>(); //stack of bricks

        private bool _isOnBridge;
        private BridgeStep _currentBridgeStep;

        public bool IsOnBridge { get => _isOnBridge; set => _isOnBridge = value; } //is the character on the bridge
        //brick count

        protected virtual void Awake()
        {
            if (this._stateMachine == null)
            {
                this._stateMachine = GetComponent<StateMachine>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            this.InitializeStates();

            this.OnInit();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bridge"))
            {
                this.IsOnBridge = true;
                this._currentBridgeStep = other.gameObject.GetComponentInChildren<BridgeStep>();
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Bridge"))
            {
                this.IsOnBridge = false;
                _currentBridgeStep = null;
            }
        }

        public bool IsOnSameColorStep()
        {
            if (this._currentBridgeStep == null || !IsOnBridge)
            {
                return true;
            }
            return this._currentBridgeStep.IsColorMatch(this.characterColor);
        }


        public virtual void SetCharacterColor(BrickColor color)
        {
            this.characterColor = color;

            if (this._renderer != null && MaterialManager.Instance != null)
            {
                var material = MaterialManager.Instance.GetMaterial(color);
                if (material != null)
                {
                    this._renderer.material = material;
                }
                else
                {
                    Debug.LogWarning($"Material for color {color} not found!");
                }
            }
            else
            {
                Debug.LogWarning("Character renderer or MaterialManager not found!");
            }
        }

        protected virtual void OnInit()
        {
            SetCharacterColor(characterColor);
        }

        protected abstract void InitializeStates();

        public virtual void PickUpBrick(Brick brick)
        {
            if (brick != null)
            {
                brickStack.Push(brick);
                if (BrickSpawner.Instance._activeBricks.ContainsKey(brick.Color))
                {
                    BrickSpawner.Instance._activeBricks[brick.Color].Remove(brick);
                }

                if (BrickSpawner.Instance._brickToSpawnPointIndex.ContainsKey(brick))
                {
                    int spawnPointIndex = BrickSpawner.Instance._brickToSpawnPointIndex[brick];
                    BrickSpawner.Instance._currentSpawnPointGenerator.SetSpawnPointAvailability(spawnPointIndex, true);
                    BrickSpawner.Instance._brickToSpawnPointIndex.Remove(brick);
                }
            }

            brick.transform.SetParent(BrickHolder);
            brick.transform.localPosition = Vector3.zero;
            brick.transform.localRotation = Quaternion.identity;

            var brickCollider = brick.GetComponent<BoxCollider>();
            var brickHeight   = brickCollider.bounds.size.y;
            brickCollider.enabled = false;


            var brickVisual = brick.transform.GetChild(0);

            brickVisual.localPosition = new Vector3(0, this.brickStack.Count * brickHeight, 0);
        }

        public virtual void PlaceBrick(Brick brick)
        {
            if (brick != null)
            {
                if (brickStack.Count > 0)
                {
                    this.brickStack.Pop();
                    brick.transform.SetParent(null);
                    BrickSpawner.Instance.RespawnBrick(brick);
                }
            }
        }

        public void ClearBrickStack()
        {
            while (this.brickStack.Count > 0)
            {
                var brick = this.brickStack.Pop();
                if (brick != null)
                {
                    brick.ReturnToPool();
                }
            }
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