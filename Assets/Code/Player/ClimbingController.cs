using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Climb
{
    public class ClimbingController : MonoBehaviour
    {
        [SerializeField, Self] public PlayerController _playerController;
        [SerializeField, Self] public EnvironmentChecker _environmentChecker;
        [SerializeField, Self] public Animator _animator;

        [Header("Actions Area")]
        [SerializeField] private List<ObjectAction> actions;
        [SerializeField] private ObjectAction jumpDownActions;

        private void Update()
        {
            if(Input.GetButton("Jump") && !_playerController.playerInAction)
            {
                OnClimbingJump();
            }

            if(!_playerController.playerInAction && _playerController._playerOnLedge && Input.GetButtonDown("Jump"))
            {
                if(_playerController.ledgeInfo.angle <= 50)
                {
                    _playerController._playerOnLedge = false;
                    StartCoroutine(OnAction(jumpDownActions));
                }
            }
        }

        private void OnClimbingJump()
        {
            ObjectObstacleInfo hitAreaData = _environmentChecker.CheckObsticle();

            if(hitAreaData.hitFound)
            {
                foreach (var action in actions)
                {
                    if(action.CheckAvailable(hitAreaData, transform))
                    {
                        StartCoroutine(OnAction(action));
                        break;
                    }
                }
            }
        }

        // void CompareTarget(ObjectAction action)
        // {
        //    _animator.MatchTarget(action.comparePos, transform.rotation, action.CompareBodyPart, new MatchTargetWeightMask(action.ComparePositionWeigth, 0), action.CompareStartTime, action.CompareEndTime);
        // }


        IEnumerator OnAction(ObjectAction action)
        {
            _playerController.SetControl(false);

            CompereTargetParameter param = null;

            if(action.AllowTargetMathing)
            {
                param = new CompereTargetParameter()
                {
                    position = action.comparePos,
                    bodyPart = action.CompareBodyPart,
                    positionWeight = action.ComparePositionWeigth,
                    startTime = action.CompareStartTime,
                    endTime = action.CompareEndTime,
                };
            }

            yield return _playerController.OnAction(action.AnimationName, param, action.RequireRotation, action.LookAtObject, action.DelayAfterAnimation);
        
   
            _playerController.SetControl(true);
        }
    }
}