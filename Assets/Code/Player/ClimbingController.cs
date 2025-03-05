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

            yield return new WaitForSeconds(animationState.length);
                       
            _playerController.SetControl(true);
            playerInAction = false;
        }
    }
}