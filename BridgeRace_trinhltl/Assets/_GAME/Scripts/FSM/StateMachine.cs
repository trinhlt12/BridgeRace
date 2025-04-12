namespace _GAME.Scripts.FSM
{
    using System.Collections.Generic;
    using UnityEngine;

    public class StateMachine
    {
        private IState                          _currentState;
        private IState                          _previousState;
        private Dictionary<System.Type, IState> _states = new Dictionary<System.Type, IState>();

        private void Update()
        {
            _currentState?.OnUpdate();
        }

        public void Initialize<T>() where T : IState
        {
            ChangeState<T>();
        }

        public void AddState<T>(T state) where T : IState
        {
            this._states[typeof(T)] = state;
        }

        public T GetState<T>() where T : IState
        {
            return (T)this._states[typeof(T)];
        }

        public void ChangeState<T>() where T : IState
        {
            if (this._currentState != null)
            {
                this._previousState = this._currentState;
                this._currentState.OnExit();
            }

            this._currentState = this._states[typeof(T)];

            this._currentState.OnEnter();

            Debug.Log($"Changed state to: {typeof(T).Name}");
        }

        public void RevertToPreviousState()
        {
            if(this._previousState == null) return;

            (this._currentState, this._previousState) = (this._previousState, this._currentState);

            this._previousState.OnExit();
            this._currentState.OnEnter();

            Debug.Log($"Reverted to previous state: {this._currentState.GetType().Name}");
        }
    }
}