using UnityEngine;

namespace Climb
{
    public class EnvironmentChecker : MonoBehaviour
    {
        [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
        [SerializeField] private float rayLength = 0.9f;
        [SerializeField] private float higthRayLength = 3f;
        [SerializeField] private LayerMask obstacleMask;

        public ObjectObstacleInfo CheckObsticle()
        {
            var hitArea = new ObjectObstacleInfo();
            
            var rayOrigin = transform.position + rayOffset;
            hitArea.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitArea.hitInfo, rayLength, obstacleMask);
            
            Debug.DrawRay(rayOrigin, transform.forward * rayLength, hitArea.hitFound ? Color.green : Color.red);

            if( hitArea.hitFound)
            {
                var hightOrigin = hitArea.hitInfo.point + Vector3.up * higthRayLength;
                hitArea.hightHitFound = Physics.Raycast(hightOrigin, Vector3.down, out hitArea.hightHitInfo, higthRayLength, obstacleMask);
                
                Debug.DrawRay(hightOrigin, Vector3.down * higthRayLength, hitArea.hightHitFound ? Color.blue : Color.red);
            }

            return hitArea;
        }
    }

    public struct ObjectObstacleInfo
    {
        public bool hitFound;
        public bool hightHitFound;
        public RaycastHit hitInfo;
        public RaycastHit hightHitInfo;
    }
}