namespace _GAME.Scripts.Character
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;
    using UnityEngine.Serialization;

    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected StateMachine _stateMachine;
        public                     BrickColor   characterColor;

        public Rigidbody rb;

        public                                       Animator           animator;
        [SerializeField]                     private Renderer           _renderer;
        public                                       int                BrickCount => this.brickStack.Count;

        public readonly                             Stack<Brick> brickStack = new Stack<Brick>(); //stack of bricks

        //brick count

        protected virtual void Awake()
        {
            if (this._stateMachine == null)
            {
                this._stateMachine = GetComponent<StateMachine>();
            }

            this.rb = this.gameObject.GetComponent<Rigidbody>();

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            this.InitializeStates();

            this.OnInit();
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

        protected abstract void OnInit();

        protected abstract void InitializeStates();

        public virtual void PickUpBrick(Brick brick)
        {
            if (brick != null)
            {
                brickStack.Push(brick);
            }
        }

        public virtual void placeBrick(Brick brick)
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
    }
}