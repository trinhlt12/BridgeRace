namespace _GAME.Scripts.FSM
{
    using System;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected StateMachine _stateMachine;
        public                     BrickColor   characterColor;

        public Rigidbody rb;

        public                   Animator animator;
        [SerializeField] private Renderer _renderer;
        public                   int      brickCount = 0; //brick count

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

        public virtual void pickUpBrick(Brick.Brick brick)
        {
            if (brick != null)
            {
                brickCount++;
            }
        }

        public virtual void placeBrick()
        {
            brickCount--;
        }
    }
}