using UnityEngine;

namespace Climb
{
    public class EnvironmentChecker : MonoBehaviour
    {
        [Header("Check Jump")]
        [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
        [SerializeField] private float rayLength = 0.9f;
        [SerializeField] private float higthRayLength = 3f;
        [SerializeField] private LayerMask obstacleMask;

        [Header("Check Ledge")]
        [SerializeField] private float ledgeRayLength = 1f;
        [SerializeField] private float ledgeRayHeightThreshold = 0.76f;
        
        [Header("Check Climb")]
        [SerializeField] private float climbRayLength = 1.6f;
        [SerializeField] private int climbRayCount = 10;
        [SerializeField] private LayerMask climbMask;

        
        private Vector3 mDirection;

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

            float ledgeOriginOffset = 0.3f;
            var ledgeOrigin = transform.position + moveDirection * ledgeOriginOffset + Vector3.up;

            if(Physics.Raycast(ledgeOrigin, Vector3.down, out RaycastHit hit, ledgeRayLength, obstacleMask))
            { 
                var groundRaycastOrigin = transform.position + moveDirection - new Vector3(0,0.1f,0);
                if(Physics.Raycast(groundRaycastOrigin, -moveDirection, out RaycastHit groundHit, 2f, obstacleMask))
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

        RaycastHit rayLedgeDownHit;

        public bool CheckClimbing(Vector3 direction, out RaycastHit climbInfo)
        {
            climbInfo = new RaycastHit();

            if(direction == Vector3.zero) return false;

            var climbOrigin = transform.position + Vector3.up * 1.5f;
            var climbOffset = new Vector3(0, 0.19f, 0);

            for (var i = 0; i < climbRayCount; i++)
            {
                Debug.DrawRay(climbOrigin + climbOffset * i, direction, Color.red);
                if(Physics.Raycast(climbOrigin + climbOffset * i, direction, out RaycastHit hit, climbRayLength, climbMask))
                {               
                    
                    Debug.DrawRay(hit.point + Vector3.up * 0.5f, Vector3.down, Color.black);
                    Physics.Raycast(hit.point + Vector3.up * 0.5f, Vector3.down, out rayLedgeDownHit, 0.7f, climbMask);
                    climbInfo = rayLedgeDownHit;
                    return true;
                }
            }

            return false;
        }

        void OnDrawGizmos()
        {
            if (rayLedgeDownHit.point != Vector3.zero) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(rayLedgeDownHit.point, 0.05f);
            }
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