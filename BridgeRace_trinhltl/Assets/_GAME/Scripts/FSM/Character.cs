namespace _GAME.Scripts.FSM
{
    using System;
    using UnityEngine;

    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected StateMachine _stateMachine;
        public        Rigidbody    rb;

        public Animator animator { get; private set; }

        //brick count
        public Color characterColor { get; private set; }

        protected virtual void Awake()
        {
            if (this._stateMachine == null)
            {
                this._stateMachine = GetComponent<StateMachine>();
            }

            rb       = this.gameObject.GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            this.InitializeStates();

            this.OnInit();
        }

        protected abstract void OnInit();

        protected abstract void InitializeStates();
    }
}