using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("Player")]        
        [SerializeField, Self] private CharacterController _charcterController;
        [SerializeField, Self] private GroundController _groundController;
        [SerializeField, Self] private Animator _animator;

        [Header("Player Movement")]
        [SerializeField, Anywhere] private CameraController _cameraController;
        [SerializeField] private float _movementSpeed = 4f;

        [Header("Player Rotation")]
        [SerializeField] private float _rotSpeed = 600f;

        [Header("Player Gravity and Collision")]
        [SerializeField] private float fallingSpeed;

        private Quaternion requireRotation;
        private Vector3 moveDir;

        private void Update() 
        {
            GravityPlayer();
            Movement();
        }

        private void GravityPlayer()
        {
            if(_groundController.onGround)
            {
                fallingSpeed = 0f;
            } else {
                fallingSpeed = Physics.gravity.y * Time.deltaTime;
            }

            var velocity = Vector3.zero * _movementSpeed;
            velocity.y = fallingSpeed;
        }

        private void Movement()
        {
            if(!_groundController.onGround) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            
            var movementInput = new Vector3(horizontal, 0, vertical).normalized;
            var movementDiraction = _cameraController.flatRotation * movementInput;
            _charcterController.Move(movementDiraction * _movementSpeed * Time.deltaTime);

            if(movementAmount > 0){
                requireRotation = Quaternion.LookRotation(movementDiraction);
            }

            movementDiraction = Vector3.zero;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, requireRotation, _rotSpeed * Time.deltaTime);
            SetAnimation(movementAmount);
        }

        private void SetAnimation(float val)
        {
            _animator.SetFloat("MovementValue", val, 0.2f, Time.deltaTime);
        }
    }
}
