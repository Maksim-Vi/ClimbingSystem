using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("Player")]        
        [SerializeField, Self] private CharacterController _charcterController;
        [SerializeField, Self] private Animator _animator;

        [Header("Player Movement")]
        [SerializeField, Anywhere] private CameraController _cameraController;
        [SerializeField] private float _movementSpeed = 4f;

        [Header("Player Rotation")]
        [SerializeField] private float _rotSpeed = 600f;

        private Quaternion requireRotation;

        private void Update() 
        {
            Movement();
        }

        private void Movement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            float movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            
            var movementInput = new Vector3(horizontal, 0, vertical).normalized;
            var movementDiraction = _cameraController.flatRotation * movementInput;

            if(movementAmount > 0){
                _charcterController.Move(movementDiraction * _movementSpeed * Time.deltaTime);
                requireRotation = Quaternion.LookRotation(movementDiraction);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, requireRotation, _rotSpeed * Time.deltaTime);
            SetAnimation(movementAmount);
        }

        private void SetAnimation(float val)
        {
            _animator.SetFloat("MovementValue", val);
        }
    }
}
