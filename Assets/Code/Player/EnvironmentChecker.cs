using UnityEngine;

namespace Climb
{
    public class EnvironmentChecker : MonoBehaviour
    {
        [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
        [SerializeField] private float rayLength = 0.9f;
        [SerializeField] private float higthRayLength = 3f;
        [SerializeField] private LayerMask obstacleMask;

        [Header("Check Ledge")]
        [SerializeField] private float ledgeRayLength = 1f;
        [SerializeField] private float ledgeRayHeightThreshold = 0.76f;


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

        public bool CheckLedge(Vector3 moveDirection, out LedgeInfo ledgeInfo)
        {
            ledgeInfo = new LedgeInfo();
            if(moveDirection == Vector3.zero) return false;

            float ledgeOriginOffset = 0.5f;
            var ledgeOrigin = transform.position + moveDirection * ledgeOriginOffset;

           
            if(Physics.Raycast(ledgeOrigin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleMask))
            { 
                Debug.DrawRay(ledgeOrigin, Vector3.down * ledgeRayLength, Color.red);

                var groundRaycastOrigin = transform.position + moveDirection - new Vector3(0,0.1f,0);
                if(Physics.Raycast(groundRaycastOrigin, -moveDirection, out RaycastHit groundHit, 2, obstacleMask))
                {
                    float LedgeHight = transform.position.y - hit.point.y;
                    if(LedgeHight > ledgeRayHeightThreshold)
                    {
                        ledgeInfo.angle = Vector3.Angle(transform.forward, groundHit.normal);
                        ledgeInfo.height = LedgeHight;
                        ledgeInfo.groundHit = groundHit;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public struct ObjectObstacleInfo
    {
        public bool hitFound;
        public bool hightHitFound;
        public RaycastHit hitInfo;
        public RaycastHit hightHitInfo;
    }

    public struct LedgeInfo
    {
        public float angle;
        public float height;
        public RaycastHit groundHit;
    }
}