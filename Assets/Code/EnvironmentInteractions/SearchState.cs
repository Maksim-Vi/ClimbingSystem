
using UnityEngine;

namespace Climb
{
    public class SearchState : EnvironmentInteractionState
    {
        public float _approachDistanceTreshold = 2.0f;
        public SearchState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(context, statKey)
        {
            Context = context;
        }

        public override void EnterState() 
        {
        }

        public override void ExitState() 
        {
        }

        public override void UpdateState() {}
        public override EnvironmentInteractionStateMachine.EnvironmentInteractionState GetNextState() 
        {
            if(CheckShouldReset())
            {
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Reset;
            }

            bool isCloseToTarget = Vector3.Distance(Context.ClosestPointOnColliderShoulder, Context.RootTransform.position) < _approachDistanceTreshold;
            bool isClosestPointOoColliderValid = Context.ClosestPointOnColliderShoulder != Vector3.positiveInfinity;

            if(isClosestPointOoColliderValid && isCloseToTarget){
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Approach;
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