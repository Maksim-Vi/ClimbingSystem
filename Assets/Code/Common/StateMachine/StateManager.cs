using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace Climb
{
    public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
    {
       protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
       protected BaseState<EState> CurrentState;

       protected bool IsTransitionState = false;

        void Start()
        {
            CurrentState.EnterState();
        }

        void Update() 
        {
            EState nextStateKey = CurrentState.GetNextState();

            if(!IsTransitionState && nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            } else if(!IsTransitionState){
                TransitionToState(nextStateKey);
            }
           
        }

        void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }

        private void TransitionToState(EState nextStateKey)
        {
            IsTransitionState = true;

            CurrentState.ExitState();
            CurrentState = States[nextStateKey];
            CurrentState.EnterState();

            IsTransitionState = false;
        }
    }
}