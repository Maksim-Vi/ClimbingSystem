using UnityEngine;

namespace Climb
{
   
    public class PlayerFallingController : StateMachineBehaviour 
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
            animator.GetComponent<PlayerController>().HashPalyerControl = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerController>().HashPalyerControl = true;
        }
    }
}