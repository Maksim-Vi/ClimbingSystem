
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Climb
{
    public class EnvironmentInteractionContext
    { 
        public enum EBodySide
        {
            RIGHT, 
            LEFT
        }
        private TwoBoneIKConstraint _leftIkConstraint;
        private TwoBoneIKConstraint _rightIkConstraint;        
        private MultiRotationConstraint _leftMultiRotationIkConstraint;
        private MultiRotationConstraint _rightMultiRotationIkConstraint;
        private CharacterController _characterController;
        private Transform _rootTransform;
        private float _characterShoulderHeigh;
        private Vector3 _leftOriginalTargetPosition;
        private Vector3 _rightOriginalTargetPosition;
        private Quaternion _originalTargetRotation;

        public EnvironmentInteractionContext(
            TwoBoneIKConstraint _leftIkConstraint, 
            TwoBoneIKConstraint _rightIkConstraint,
            MultiRotationConstraint _leftMultiRotationIkConstraint,
            MultiRotationConstraint _rightMultiRotationIkConstraint,
            CharacterController _characterController,
            Transform _rootTransform
        ){
            this._leftIkConstraint = _leftIkConstraint;
            this._rightIkConstraint = _rightIkConstraint;
            this._leftMultiRotationIkConstraint = _leftMultiRotationIkConstraint;
            this._rightMultiRotationIkConstraint = _rightMultiRotationIkConstraint;
            this._characterController = _characterController;
            this._rootTransform = _rootTransform;

            _leftOriginalTargetPosition = _leftIkConstraint.data.target.transform.localPosition;
            _rightOriginalTargetPosition = _rightIkConstraint.data.target.transform.localPosition;
            _originalTargetRotation = _leftIkConstraint.data.target.transform.rotation;

            _characterShoulderHeigh = _leftIkConstraint.data.root.transform.position.y;
            SetCurrentSide(Vector3.positiveInfinity);
        }

        public TwoBoneIKConstraint LeftIkConstraint => _leftIkConstraint;
        public TwoBoneIKConstraint RightIkConstraint => _rightIkConstraint;
        public MultiRotationConstraint LeftMultiRotationIkConstraint => _leftMultiRotationIkConstraint;
        public MultiRotationConstraint RightMultiRotationIkConstraint => _rightMultiRotationIkConstraint;
        public CharacterController CharacterController => _characterController;
        public Transform RootTransform => _rootTransform;
        public float CharacterShoulderHeigh => _characterShoulderHeigh;

        public Collider CurrentIntersectingCollider {get; set; }
        public TwoBoneIKConstraint CurrentIkConstraint {get; private set; }
        public MultiRotationConstraint CurrentMultiRotationIkConstraint {get; private set; }
        public Vector3 CurrentOriginalTargetPosition {get; private set; }
        public Quaternion CurrentOriginalTargetRotation {get; private set; }
        public Transform CurrentIkTargetTransform {get; private set; }
        public Transform CurrentShoulderTransform {get; private set; }
        public EBodySide CurrentBodySide {get; private set; }
        public Vector3 ClosestPointOnColliderShoulder {get; set;} = Vector3.positiveInfinity;
        public float InteractionPointYOffset {get; set;} = 0;
        public float ColliderCenterY {get; set;}
        public float LowestDictance {get; set;} = Mathf.Infinity;

        public void SetCurrentSide(Vector3 positionToCheck)
        {
            Vector3 leftShoulder = _leftIkConstraint.data.root.transform.position;
            Vector3 rightShoulder = _rightIkConstraint.data.root.transform.position;

            bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);

            if(isLeftCloser){
                CurrentIkConstraint = _leftIkConstraint;
                CurrentMultiRotationIkConstraint = _leftMultiRotationIkConstraint;
                CurrentOriginalTargetPosition = _leftOriginalTargetPosition;
                CurrentBodySide = EBodySide.LEFT;
            } else {
                CurrentIkConstraint = _rightIkConstraint;
                CurrentMultiRotationIkConstraint = _rightMultiRotationIkConstraint;
                CurrentOriginalTargetPosition = _rightOriginalTargetPosition;
                CurrentBodySide = EBodySide.RIGHT;
            }

            CurrentOriginalTargetRotation = _originalTargetRotation;
            CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
            CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
        }
    }
}