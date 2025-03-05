using UnityEngine;

namespace Climb
{
    [CreateAssetMenu(menuName = "ClambingAction/Create Climbing Action")]
    public class ObjectAction : ScriptableObject
    {        
        [Header("animation name")]
        [SerializeField] string animationName;
        [Header("set min amd max value to detect object size")]
        [SerializeField] float minHeight;
        [SerializeField] float maxHeight;
        [SerializeField] bool lookAtObject;

        public Quaternion RequireRotation { get ; set;}

        public bool CheckAvailable(ObjectObstacleInfo hitData, Transform player)
        {
            float checkHeight = hitData.hightHitInfo.point.y - player.position.y;
            
            if(checkHeight < minHeight || checkHeight > maxHeight) 
                return false;

            if(lookAtObject)
            {
                RequireRotation = Quaternion.LookRotation(-hitData.hitInfo.normal);
            }

            return true;
        }

        public string AnimationName => animationName;
        public bool LookAtObject => lookAtObject;
    }
}