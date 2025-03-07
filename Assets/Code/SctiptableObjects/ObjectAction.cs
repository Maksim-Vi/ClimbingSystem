using UnityEngine;

namespace Climb
{
    [CreateAssetMenu(menuName = "ClambingAction/Create Climbing Action")]
    public class ObjectAction : ScriptableObject
    {        
        
        [Header("animation name")]
        [SerializeField] string animationName;
        [SerializeField] string objectTag;

        [Header("set min amd max value to detect object size")]
        [SerializeField] float minHeight;
        [SerializeField] float maxHeight;

        [Header("Rotation if looking to Object")]
        [SerializeField] bool lookAtObject;
        [SerializeField] float delayAfterAnimation = 0f;

        [Header("target math")]
        [SerializeField] bool allowTargetMathing = true;
        [SerializeField] AvatarTarget compareBodyPart; 
        [SerializeField] float compareStartTime; 
        [SerializeField] float compareEndTime; 
        [SerializeField] Vector3 comparePositionWeigth = new Vector3(0,1,1); 


        public Quaternion RequireRotation { get ; set;}
        public Vector3 comparePos {get; set;}

        public bool CheckAvailable(ObjectObstacleInfo hitData, Transform player)
        {
            if(!string.IsNullOrEmpty(objectTag) && hitData.hitInfo.transform.tag != objectTag)
            {
                return false;
            }

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
        public float DelayAfterAnimation => delayAfterAnimation;
        public Vector3 ComparePositionWeigth => comparePositionWeigth;
    }
}