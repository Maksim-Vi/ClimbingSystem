using System.Collections;
using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        public bool pControl => playerControl;
        public float rotSpeed => _rotSpeed;
        public bool playerInAction => _playerInAction;
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
        private bool _playerInAction = false;
        [HideInInspector] public bool _playerOnLedge {get; set;} = false;
        private LedgeInfo _ledgeInfo {get; set; }
        private Vector3 moveDir;
        private Vector3 velocity;
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

            velocity = Vector3.zero;

            if(_groundController.onGround)
            {
                fallingSpeed = 0f;
                velocity = moveDir * _movementSpeed;
                CheckLedgeMovement();

                SetAnimation(velocity.magnitude / _movementSpeed);
            } else {
                fallingSpeed += Physics.gravity.y * Time.deltaTime;
                velocity = transform.forward * _movementSpeed / 2;
            }
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
            } else {
                moveDir = Vector3.zero;
            }
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
                    velocity = Vector3.zero;         
                    moveDir = Vector3.zero;
                    return;
                } 
            }
        }

        public void SetControl(bool hasControl)
        {
            playerControl = hasControl;

            if(!hasControl)
            {
                _animator.SetFloat("MovementValue", 0f);
            }
        }

        public IEnumerator OnAction(string animationName, CompereTargetParameter param, Quaternion requireRotation, bool lookAtObject = false, float DelayAfterAnimation = 0f)
        {
            _playerInAction = true;
            SetControl(false);

            _animator.CrossFade(animationName, 0f);

            yield return null;

            var animationState = _animator.GetNextAnimatorStateInfo(0);

            if(!animationState.IsName(animationName))
            {
                Debug.LogWarning("Wrong animation name =>" + animationName);
            }


            float time = 0f;

            while(time <= animationState.length)
            {
                time += Time.deltaTime;

                if(lookAtObject)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, requireRotation, rotSpeed * Time.deltaTime);
                }

                if(param != null)
                {
                    CompareTarget(param);
                }

                if(_animator.IsInTransition(0) && time > 0.7f)
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(DelayAfterAnimation);
                       
            SetControl(true);
            _playerInAction = false;
        }

        void CompareTarget(CompereTargetParameter param)
        {
           _animator.MatchTarget(param.position, transform.rotation, param.bodyPart, new MatchTargetWeightMask(param.positionWeight, 0), param.startTime, param.endTime);
        }


        public bool HashPalyerControl
        {
            get => playerControl;
            set => playerControl = value;
        }
    }
}

public class CompereTargetParameter
{
    public Vector3 position;
    public AvatarTarget bodyPart;
    public Vector3 positionWeight;
    public float startTime;
    public float endTime;

}
