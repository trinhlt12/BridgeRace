namespace _GAME.Scripts.FSM
{
    using System.Collections.Generic;
    using UnityEngine;

    public class StateMachine : MonoBehaviour
    {
        private IState                          _currentState;
        private IState                          _previousState;
        private Dictionary<System.Type, IState> _states = new Dictionary<System.Type, IState>();

        private void Update()
        {
            _currentState?.OnUpdate();
        }

        private void FixedUpdate()
        {
            _currentState?.OnFixedUpdate();
        }

        public void Initialize<T>() where T : IState
        {
            ChangeState<T>();
        }

        public void AddState<T>(T state) where T : IState
        {
            if (state == null)
            {
                Debug.LogError($"Attempted to add null state of type {typeof(T).Name}");
                return;
            }

            this._states[typeof(T)] = state;
        }

        public T GetState<T>() where T : IState
        {
            return (T)this._states[typeof(T)];
        }

        public void ChangeState<T>() where T : IState
        {
            if (!_states.TryGetValue(typeof(T), out IState newState))
            {
                Debug.LogError($"State {typeof(T).Name} not found in state machine");
                return;
            }

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