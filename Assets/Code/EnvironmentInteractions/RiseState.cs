
using UnityEngine;

namespace Climb
{
    public class RiseState : EnvironmentInteractionState
    {        
        float _elapsedTime = 0.0f;
        float _lerpDuration = 5.0f;
        float _rideWeight = 1f;
        float _rotationSpeed = 1000f;
        public Quaternion _expectedHandRotation;
        public float _maxDistance = .5f;
        protected LayerMask _interactableLayerMask = LayerMask.GetMask("Interectable");

        public float _toughtDictanceTreshold = .05f;
        public float _toughtTimeTreshold = 1f;

        public RiseState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(context, statKey)
        {
            Context = context;  
        }

        public override void EnterState() 
        {
            _elapsedTime = 0.0f;
        }

        public override void ExitState() 
        {
        }

        public override void UpdateState() 
        {
            _elapsedTime += Time.deltaTime;
            Context.InteractionPointYOffset = Mathf.Lerp(Context.InteractionPointYOffset, Context.ClosestPointOnColliderShoulder.y, _elapsedTime / _lerpDuration);
            
            // set weight hend up to dot
            Context.CurrentIkConstraint.weight = Mathf.Lerp(Context.CurrentIkConstraint.weight, _rideWeight, _elapsedTime / _lerpDuration);
            Context.CurrentMultiRotationIkConstraint.weight = Mathf.Lerp(Context.CurrentMultiRotationIkConstraint.weight, _rideWeight, _elapsedTime / _lerpDuration);
            
            // set rotation hand to wall
            CalcExpectedHandRotation();
            Context.CurrentIkTargetTransform.rotation = Quaternion.RotateTowards(Context.CurrentIkTargetTransform.rotation, _expectedHandRotation, Time.deltaTime * _rotationSpeed);
        }

        public override EnvironmentInteractionStateMachine.EnvironmentInteractionState GetNextState() 
        {
            if(CheckShouldReset())
            {
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Reset;
            }

            bool isDictanseTought = Vector3.Distance(Context.CurrentIkTargetTransform.position, Context.ClosestPointOnColliderShoulder) < _toughtDictanceTreshold;
            if(isDictanseTought && _elapsedTime >= _toughtTimeTreshold)
            {
                return EnvironmentInteractionStateMachine.EnvironmentInteractionState.Touch;
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

        private void CalcExpectedHandRotation()
        {
            Vector3 startPos = Context.CurrentShoulderTransform.position;
            Vector3 endPos = Context.ClosestPointOnColliderShoulder;
            Vector3 direction = (endPos - startPos).normalized;

            if(endPos == Vector3.positiveInfinity) return;

            RaycastHit hit;
            if(Physics.Raycast(startPos, direction, out hit, _maxDistance, _interactableLayerMask))
            {
                Vector3 surfaceNormal = hit.normal;
                Vector3 targetForward = -surfaceNormal;
                _expectedHandRotation = Quaternion.LookRotation(targetForward, Vector3.up);


            }
        }
    }
}