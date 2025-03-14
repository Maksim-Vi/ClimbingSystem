
using UnityEngine;

namespace Climb
{
    public abstract class EnvironmentInteractionState : BaseState<EnvironmentInteractionStateMachine.EnvironmentInteractionState>
    {
        protected EnvironmentInteractionContext Context;
        private float _movingOffset = .005f;
        private bool _shouldReset = false;

        public EnvironmentInteractionState(EnvironmentInteractionContext context, EnvironmentInteractionStateMachine.EnvironmentInteractionState statKey) : base(statKey)
        {
            Context = context;
        }

        protected bool CheckShouldReset()
        {
            if(_shouldReset)
            {
                Context.LowestDictance = Mathf.Infinity;
                _shouldReset = false;
                return true;
            }

            bool isPlayerStopped = Context.CharacterController.velocity.magnitude > 0.1f;
            bool isPlayerJump =  Mathf.Round(Context.CharacterController.velocity.y) >= 1;
            bool isMovingAway = CheckIsMovingAway();
            bool isBadAngle = CheckIsBadDeg();

            if(isPlayerStopped || isPlayerJump || isMovingAway || isBadAngle) 
            {
                Context.LowestDictance = Mathf.Infinity;
                return true;
            }

            return false;
        }

        private bool CheckIsMovingAway()
        {
            bool isSearchingInteraction = Context.CurrentIntersectingCollider == null;
            if(isSearchingInteraction) return false;

            // calc dictance from player to target and check is dist les then Low dist
            float currentDictanceToTarget = Vector3.Distance(Context.RootTransform.position, Context.ClosestPointOnColliderShoulder);
            bool isGettingClosetToTarget = currentDictanceToTarget <= Context.LowestDictance;

            if(isGettingClosetToTarget)
            {
                Context.LowestDictance = currentDictanceToTarget;
                return false;
            }

            bool isMoveAwayFromTarget = currentDictanceToTarget > Context.LowestDictance + _movingOffset;
            if(isMoveAwayFromTarget)
            {
                Context.LowestDictance = Mathf.Infinity;
                return true;
            }

            return false;
        }

        private bool CheckIsBadDeg()
        {
            bool isSearchingInteraction = Context.CurrentIntersectingCollider == null;
            if(isSearchingInteraction) return false;

            Vector3 targetDirection = Context.ClosestPointOnColliderShoulder - Context.CurrentShoulderTransform.position;
            Vector3 shoulderDirection = Context.CurrentBodySide == EnvironmentInteractionContext.EBodySide.RIGHT 
                ? Context.RootTransform.right 
                : -Context.RootTransform.right;

            // the function returns the scalar product of two vectors, which determines the angular similarity between them
            // where 1 - 0deg; 0 - 90deg; -1 - 180deg;
            float dotProduct = Vector3.Dot(shoulderDirection, targetDirection.normalized);

            bool isBadAngle = dotProduct < 0;
            
            return isBadAngle;
        }

        private Vector3 GetCloserPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
        {
            return intersectingCollider.ClosestPoint(positionToCheck);
        }

        protected void StartIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(
                intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Interectable") &&
                Context.CurrentIntersectingCollider == null
            ){
                Context.CurrentIntersectingCollider = intersectingCollider;
                Vector3 closestPointFromRoot = GetCloserPointOnCollider(intersectingCollider, Context.RootTransform.position);
                Context.SetCurrentSide(closestPointFromRoot);

                SetIkTargetPosition();
            }
        }

        protected void UpdateIKTargetPosition(Collider intersectingCollider)
        {
            if(intersectingCollider == Context.CurrentIntersectingCollider)
            {
                SetIkTargetPosition();
            }
        }

        protected void ResetIKTargetPositionTracking(Collider intersectingCollider)
        {
            if(intersectingCollider == Context.CurrentIntersectingCollider)
            {
                Context.CurrentIntersectingCollider = null;
                Context.ClosestPointOnColliderShoulder = Vector3.positiveInfinity;
                _shouldReset = true;
            }
        }

        private void SetIkTargetPosition()
        {
            Vector3 shoulderPosition = new Vector3(
                Context.CurrentShoulderTransform.position.x,
                Context.CharacterShoulderHeigh,
                Context.CurrentShoulderTransform.position.z
            );

            Context.ClosestPointOnColliderShoulder = GetCloserPointOnCollider(Context.CurrentIntersectingCollider, shoulderPosition);

            // direction from point to player
            Vector3 reyDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderShoulder;
            Vector3 normalizedReyDirection = reyDirection.normalized;
            // offset from wall to hand
            float offsetDistance = .05f;
            Vector3 offset = normalizedReyDirection * offsetDistance;

            Vector3 offsetPosition = Context.ClosestPointOnColliderShoulder + offset;
            Context.CurrentIkTargetTransform.position = new Vector3(
                offsetPosition.x,
                Context.InteractionPointYOffset,
                offsetPosition.z
            ); // set target position of dot on wall with offset where InteractionPointYOffset this is Mathf.Lerp(Context.InteractionPointYOffset, Context.ColliderCenterY, _elipsedTime / _lerpDuration);
        }
    }
}