namespace _GAME.Scripts.FSM
{
    using System;
    using UnityEngine;

    public abstract class Character : MonoBehaviour
    {
        protected StateMachine _stateMachine;
        public    Rigidbody    rb { get; private set; }

        public Animator animator { get; private set; }

        //brick count
        public Color characterColor { get; private set; }

        protected virtual void Awake()
        {
            this.OnInit();
        }

        protected virtual void OnInit()
        {
            this._stateMachine = GetComponent<StateMachine>();

            rb       = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        protected abstract void InitializeStates();
    }
}