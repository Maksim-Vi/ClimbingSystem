using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class GroundController : MonoBehaviour
    {
        [SerializeField, Self] public PlayerController _playerController;
        [SerializeField] public float groundCheckRadius = 0.3f;
        public Vector3 groundCheckOffset;
        public LayerMask groundLayer;
        public bool onGround = false;

        private void Update()
        {
            CheckIsGround();
        }

        public void CheckIsGround()
        {
            if(!_playerController.pControl) return;

            onGround = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
        }
    }
}