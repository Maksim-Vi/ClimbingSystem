
using UnityEngine;

namespace Climb
{
    public class TouchState : EnvironmentInteractionState
    {
        float _elapsedTime = 0.0f;
        float _resetTreshold = .5f;

        public TouchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(context, statKey)
        {
            Context = context;  
        }
        
        public override void EnterState() 
        {
            _elapsedTime = 0.0f;
        }

        public override void ExitState() {}
        public override void UpdateState() 
        {
            _elapsedTime += Time.deltaTime;

        }

        public override EnvironmentInteractionStateMachine.EnvironmentInteractionState GetNextState() 
        {
            if(_elapsedTime > _resetTreshold || CheckShouldReset())
            {
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Reset;
            }

            return StateKey;
        }

        public override void OnTriggerEnter(Collider other) 
        {
            StartIKTargetPositionTracking(other);
        }

        public override void OnTriggerStay(Collider other) 
        {
            UpdateIKTargetPosition(other);
        }

        public override void OnTriggerExit(Collider other) 
        {
            ResetIKTargetPositionTracking(other);
        }
    }
}