using System.Collections;
using KBCore.Refs;
using UnityEngine;


namespace Climb
{
    public class ClimbingController : MonoBehaviour
    {
        [SerializeField, Self] private EnvironmentChecker _environmentChecker;
        [SerializeField, Self] private PlayerController _playerController;
        [SerializeField, Self] private Animator _animator;

        [SerializeField] private float InOutValue;
        [SerializeField] private float UpDownValue;
        [SerializeField] private float LeftRightValue;

        private void Update() 
        {
            if(!_playerController._playerHanging)
            {
                if(Input.GetButton("Jump") && !_playerController._playerInAction)
                {
                    if(_environmentChecker.CheckClimbing(transform.forward, out RaycastHit climbInfo))
                    {
                        _playerController.SetControl(false);
                        StartCoroutine(ClimpToLedge("IdleToClimb", climbInfo, 0.40f, 54f));
                    }
                }
            } else {
                // climb actions
            }
        }

        IEnumerator ClimpToLedge(string animationName, RaycastHit ledgePoint, float startTime, float endTime)
        {
            var param = new CompereTargetParameter()
            {
                position = SetHandPosition(ledgePoint),
                bodyPart = AvatarTarget.RightHand,
                positionWeight = new Vector3(0,1,1),
                startTime = startTime,
                endTime = endTime,
            };

            var RequireRotation = Quaternion.LookRotation(-ledgePoint.transform.forward);
            
            yield return _playerController.OnAction(animationName, param, RequireRotation, true);

            // _playerController.SetControl(true);
            _playerController._playerHanging = true;

        }

        Vector3 SetHandPosition(RaycastHit ledge)
        {
            InOutValue = 0.12f;
            UpDownValue = 0.06f;
            LeftRightValue = 0.20f;
            Vector3 handPos = ledge.point + transform.forward * InOutValue + Vector3.up * UpDownValue;

            //Vector3 handPos = ledge.point + ledge.transform.forward * InOutValue + Vector3.up * UpDownValue - ledge.transform.right * LeftRightValue;

            return ledge.point;
        }
    }
}
