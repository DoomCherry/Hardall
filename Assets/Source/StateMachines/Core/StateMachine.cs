using System;
using System.Collections.Generic;

namespace StateMachineCore
{
    public class StateMachine
    {
        private List<IState> _states = new List<IState>();
        private IState _currentState;
        private bool _isStateStart = false;




        public IState CurrentState
        {
            get => _currentState;
        }
        public IReadOnlyList<IState> States
        {
            get => _states;
        }




        public void Initialize<T>() where T : IState
        {
            ForceStartState<T>(OnStarted);


            void OnStarted()
            {
                _isStateStart = true;
            }
        }

        public virtual void Tick()
        {
            if (_isStateStart)
                _currentState.Tick();
        }

        public void AddState<T>(T state) where T : IState
        {
            if (state == null)
                throw new System.NullReferenceException($"State is not equal a null!");

            if (GetState<T>() == null)
                _states.Add(state);
            else
                throw new System.ArgumentException($"{typeof(T)} this state is contain in state machine.");
        }

        public void StartState<T>() where T : IState
        {
            if (_currentState != null)
            {
                EndCurrentState(OnEnded);
            }
            else
            {
                ForceStartState<T>(OnStarted);
            }


            void OnStarted()
            {
                _isStateStart = true;
            }

            void OnEnded()
            {
                ForceStartState<T>(OnStarted);
            }
        }

        protected void ForceStartState<T>(Action callback) where T : IState
        {
            _isStateStart = false;
            _currentState = GetState<T>();
            _currentState.Start(callback);
        }

        protected void EndCurrentState(Action callback)
        {
            _currentState.End(callback);
        }

        private IState GetState<T>() where T : IState
        {
            foreach (var item in _states)
            {
                if (item.GetType() == typeof(T))
                    return item;
            }

            return null;
        }
    }
}
