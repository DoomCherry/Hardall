using System;

namespace StateMachineCore
{
    public interface IState
    {
        event Action OnStartState;
        event Action OnEndState;




        int StateCycleCount { get; }




        void Start(Action callback);
        void Tick();
        void End(Action callback);
    }
}
