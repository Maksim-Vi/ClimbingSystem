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

        [Header("Rotation if looking to Object")]
        [SerializeField] bool lookAtObject;

        [Header("target math")]
        [SerializeField] bool allowTargetMathing = true;
        [SerializeField] AvatarTarget compareBodyPart; 
        [SerializeField] float compareStartTime; 
        [SerializeField] float compareEndTime; 


        public Quaternion RequireRotation { get ; set;}
        public Vector3 comparePos {get; set;}

        public bool CheckAvailable(ObjectObstacleInfo hitData, Transform player)
        {
            float checkHeight = hitData.hightHitInfo.point.y - player.position.y;
            
            if(checkHeight < minHeight || checkHeight > maxHeight) 
                return false;

            if(lookAtObject)
            {
                RequireRotation = Quaternion.LookRotation(-hitData.hitInfo.normal);
            }

            if(allowTargetMathing)
            {
                comparePos = hitData.hightHitInfo.point;

            }

            return true;
        }

        public string AnimationName => animationName;
        public bool LookAtObject => lookAtObject;
        public bool AllowTargetMathing => allowTargetMathing;
        public AvatarTarget CompareBodyPart => compareBodyPart;
        public float CompareStartTime => compareStartTime;
        public float CompareEndTime => compareEndTime;
    }
}