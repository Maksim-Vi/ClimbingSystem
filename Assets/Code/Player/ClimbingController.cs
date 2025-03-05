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

        private bool playerInAction = false;

        private void Update()
        {
            if(Input.GetButton("Jump") && !playerInAction)
            {
                OnClimbingJump();
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
                        StartCoroutine(ClimbingAction(action));
                        break;
                    }
                }
            }
        }

        void CompareTarget(ObjectAction action)
        {
           _animator.MatchTarget(action.comparePos, transform.rotation, action.CompareBodyPart, new MatchTargetWeightMask(new Vector3(0,1,0), 0), action.CompareStartTime, action.CompareEndTime);
        }

        IEnumerator ClimbingAction(ObjectAction action)
        {
            playerInAction = true;
            _playerController.SetControl(false);

            _animator.CrossFade(action.AnimationName, 0f);

            yield return null;

            var animationState = _animator.GetNextAnimatorStateInfo(0);

            if(!animationState.IsName(action.AnimationName))
            {
                Debug.LogWarning("Wrong animation name =>" + action.AnimationName);
            }

            //yield return new WaitForSeconds(animationState.length);

            float time = 0f;

            while(time <= animationState.length)
            {
                time += Time.deltaTime;

                if(action.LookAtObject)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, action.RequireRotation, _playerController.rotSpeed * Time.deltaTime);
                }

                if(action.AllowTargetMathing)
                {
                    CompareTarget(action);
                }

                yield return null;
            }
                       
            _playerController.SetControl(true);
            playerInAction = false;
        }
    }
}