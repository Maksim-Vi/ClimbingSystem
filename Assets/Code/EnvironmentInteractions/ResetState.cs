
using UnityEngine;

namespace Climb
{
    public class ResetState : EnvironmentInteractionState
    {
        float _elapsedTime = 0.0f;
        float _resetDuration = 2.0f;
        float _lerpDuration = 10.0f;
        float _rotetionSpeed = 500f;

        public ResetState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(context, statKey)
        {
            Context = context;
        }

        public override void EnterState() 
        {
            _elapsedTime = 0.0f;
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderShoulder = Vector3.positiveInfinity;
        }

        public override void ExitState() 
        {
        }

        public override void UpdateState() 
        {
            _elapsedTime += Time.deltaTime;
            Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset, Context.ColliderCenterY, _elapsedTime / _lerpDuration);
                
            
            // back to default data
            Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, 0, _elapsedTime / _lerpDuration);
            Context.CurrentMultiRotationIkConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationIkConstraint.weight, 0, _elapsedTime / _lerpDuration);
            
            Context.CurrentIkTargetTransform.localPosition = Vector3.Lerp(Context.CurrentIkTargetTransform.localPosition, Context.CurrentOriginalTargetPosition, _elapsedTime / _lerpDuration);
            Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, Context.CurrentOriginalTargetRotation, _rotetionSpeed * Time.deltaTime);
        }

        public override EnvironmentInteractionStateMachine.EnvironmentInteractionState GetNextState() 
        {
            bool isMoving = Context.CharacterController.velocity != Vector3.zero;
            if(_elapsedTime >= _resetDuration && isMoving){
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Search;
            }

            return StateKey;
        }

        public override void OnTriggerEnter(Collider other) {}
        public override void OnTriggerStay(Collider other) {}
        public override void OnTriggerExit(Collider other) {}
    }
}