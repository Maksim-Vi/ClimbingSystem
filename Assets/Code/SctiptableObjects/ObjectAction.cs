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

        public bool CheckAvailable(ObjectObstacleInfo hitInfo, Transform player)
        {
            float checkHeight = hitInfo.hightHitInfo.point.y - player.position.y;

            return (checkHeight < minHeight || checkHeight > maxHeight) ? false : true;
        }

        public string AnimationName => animationName;
    }
}