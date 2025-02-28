using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField, Self] private CharacterController _charcterController;
        [SerializeField, Anywhere] private CameraController _cameraController;
        [SerializeField] private float _movementSpeed = 4f;

        private void Update() 
        {
            Movement();
        }

        private void Movement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float movementAmount = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            var movementInput = new Vector3(horizontal, 0, vertical).normalized;
            var movementDiraction = _cameraController.flatRotation * movementInput;

            if(movementAmount > 0){
                _charcterController.Move(movementDiraction * _movementSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(movementDiraction);
            }
        }
    }
}
