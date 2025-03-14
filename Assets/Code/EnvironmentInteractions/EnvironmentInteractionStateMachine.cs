using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

namespace Climb
{
    public class EnvironmentInteractionStateMachine : StateManager<EnvironmentInteractionStateMachine.EnvironmentInteractionState>
    {
        public enum EnvironmentInteractionState
        {
            Search,
            Approach,
            Rise,
            Touch,
            Reset
        }
        [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
        [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;        
        [SerializeField] private MultiRotationConstraint _leftMultiRotationIkConstraint;
        [SerializeField] private MultiRotationConstraint _rightMultiRotationIkConstraint;
        [SerializeField] private CharacterController _characterController;

        private EnvironmentInteractionContext _context;

        void Awake()
        {
            ValidateConstraints();
            _leftIkConstraint.weight = 0;
            _rightIkConstraint.weight = 0;           
            _leftMultiRotationIkConstraint.weight = 0;
            _rightMultiRotationIkConstraint.weight = 0;
            _context = new EnvironmentInteractionContext(
                _leftIkConstraint,
                _rightIkConstraint,
                _leftMultiRotationIkConstraint, 
                _rightMultiRotationIkConstraint, 
                _characterController,
                transform.root
            );
            InitializeStates();
            CunstructEnvironmentDetectionCollider();
        }

        private void ValidateConstraints()
        {
            Assert.IsNotNull(_leftIkConstraint, "Left IK const. is empty");
            Assert.IsNotNull(_rightIkConstraint, "Right IK const. is empty");            
            Assert.IsNotNull(_leftMultiRotationIkConstraint, "Left Rotation IK const. is empty");
            Assert.IsNotNull(_rightMultiRotationIkConstraint, "Right Rotation IK const. is empty");
            Assert.IsNotNull(_characterController, "CharacterController is empty");
        }

        private void InitializeStates()
        {
            States.Add(EnvironmentInteractionState.Reset, new ResetState(_context, EnvironmentInteractionState.Reset));
            States.Add(EnvironmentInteractionState.Approach, new ApproachState(_context, EnvironmentInteractionState.Approach));
            States.Add(EnvironmentInteractionState.Rise, new RiseState(_context, EnvironmentInteractionState.Rise));
            States.Add(EnvironmentInteractionState.Search, new SearchState(_context, EnvironmentInteractionState.Search));
            States.Add(EnvironmentInteractionState.Touch, new TouchState(_context, EnvironmentInteractionState.Touch));

            CurrentState = States[EnvironmentInteractionState.Reset];
        }

        private void CunstructEnvironmentDetectionCollider()
        {
            float wingspon = _characterController.height;

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(wingspon, wingspon, wingspon);

            float yPos = _characterController.center.y + (.25f * wingspon);
            float zPos =  _characterController.center.z + (.5f * wingspon);
            boxCollider.center = new Vector3(_characterController.center.x, yPos, zPos);
            boxCollider.isTrigger = true;

            _context.ColliderCenterY = _characterController.center.y;
        }

        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.red;

            if(_context != null && _context.ClosestPointOnColliderShoulder != null)
            {
                Gizmos.DrawSphere(_context.ClosestPointOnColliderShoulder, .03f);
            }
        }
    }
}