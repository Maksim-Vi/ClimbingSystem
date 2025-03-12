using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        public bool pControl => playerControl;
        public float rotSpeed => _rotSpeed;
        public LedgeInfo ledgeInfo => _ledgeInfo;

        [Header("Player")]        
        [SerializeField, Self] private CharacterController _charcterController;
        [SerializeField, Self] private GroundController _groundController;
        [SerializeField, Self] private EnvironmentChecker _environmentChecker;
        [SerializeField, Self] private Animator _animator;

        [Header("Player Movement")]
        [SerializeField, Anywhere] private Transform _camera;
        [SerializeField] private float _movementSpeed = 4f;

        [Header("Player Rotation")]
        [SerializeField] private float _rotSpeed = 600f;

        [Header("Player Gravity and Collision")]
        [SerializeField] private float fallingSpeed;

        private bool playerControl = false;
        [HideInInspector] public bool _playerOnLedge {get; set;} = false;
        private LedgeInfo _ledgeInfo {get; set; }
        private Vector3 moveDir;
        private Vector3 velocity;
        private bool _playerCanMove = true;
        private float turnSmoothVelocity;

        private void Start() 
        {
            playerControl = true;
        }

        private void Update() 
        {    
            Movement();
            GravityPlayer();
        }

        private void GravityPlayer()
        {
            if(!playerControl) return;

            if(_groundController.onGround)
            {
                fallingSpeed = 0f;
                CheckLedgeMovement();
            } else {
                fallingSpeed += Physics.gravity.y * Time.deltaTime;
            }

            velocity = moveDir * _movementSpeed;
            velocity.y = fallingSpeed;

            _animator.SetBool("onGround", _groundController.onGround);
        }

        private void Movement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            
            var movementInput = new Vector3(horizontal, 0, vertical).normalized;

            if(playerControl && movementInput.magnitude >= 0.1f)
            {
                // calc half Angle of 2 directions, and add Angle of camera y. this give me direction Angle of camera view
                float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                     
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
               
               _charcterController.Move(velocity * Time.deltaTime);
            }

            SetAnimation(movementAmount);
        }

        private void SetAnimation(float val)
        {
            _animator.SetFloat("MovementValue", val, 0.2f, Time.deltaTime);
        }

        void CheckLedgeMovement()
        {
            if(!_groundController.onGround || !playerControl) return;

            _playerOnLedge = _environmentChecker.CheckLedge(moveDir.normalized, out LedgeInfo ledgeInfo);
            if(_playerOnLedge)
            {
                _ledgeInfo = ledgeInfo;
                
                float angle = Vector3.Angle(_ledgeInfo.groundHit.normal, moveDir);

                if(angle < 90)
                {
                    Debug.Log("On End Ledge");
                    _playerCanMove = false;
                    velocity = Vector3.zero;         
                    moveDir = Vector3.zero;
                    return;
                } 
            }

            _playerCanMove = true;
        }

        public void SetControl(bool hasControl)
        {
            playerControl = hasControl;
            // _charcterController.enabled = hasControl;

            if(!hasControl)
            {
                _animator.SetFloat("MovementValue", 0f);
                //requireRotation = transform.rotation;
            }
        }
    }
}
