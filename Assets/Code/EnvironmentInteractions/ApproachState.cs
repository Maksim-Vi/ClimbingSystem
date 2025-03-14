
using UnityEngine;

namespace Climb
{
    public class ApproachState : EnvironmentInteractionState
    {  
        public float _ellapsedTime = 0.0f;
        public float _lerpDuration = 5.0f;
        public float _approachWeight = .5f;
        public float _approachRotationWeight = .75f;
        public float _rotationSpeed = 500f;
        public float _riseDictanceTreshold = .5f;
        public float _approachDuration = 5.0f;

        public ApproachState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(context, statKey)
        {
            Context = context;
        }

        public override void EnterState() 
        {
            _ellapsedTime = 0.0f;
        }

        public override void ExitState() 
        {
            Context.CurrentMultiRotationIkConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationIkConstraint.weight, 0, _ellapsedTime / _lerpDuration);
            Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, 0, _ellapsedTime / _lerpDuration);
        }

        public override void UpdateState() 
        {
            _ellapsedTime += Time.deltaTime;

            // create rot. with z-axis down toward the ground
            Quaternion expectedGroundRotation = Quaternion.LookRotation(-Vector3.up, Context.RootTransform.forward);
            Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, expectedGroundRotation, _rotationSpeed * Time.deltaTime); 
            
            Context.CurrentMultiRotationIkConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationIkConstraint.weight, _approachRotationWeight, _ellapsedTime / _lerpDuration);
            Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, _approachWeight, _ellapsedTime / _lerpDuration);
        }

        public override EnvironmentInteractionStateMachine.EnvironmentInteractionState GetNextState() 
        {
            bool isOverStateLiveDuration = _ellapsedTime >= _approachDuration;
            if(isOverStateLiveDuration || CheckShouldReset()){
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Reset;
            }

            bool isWithinArmReach = Vector3.Distance(Context.ClosestPointOnColliderShoulder, Context.CurrentShoulderTransform.position) < _riseDictanceTreshold;
            bool isClosestPointOnColiderReal = Context.ClosestPointOnColliderShoulder != Vector3.positiveInfinity;

            if(isWithinArmReach && isClosestPointOnColiderReal){
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Rise;
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