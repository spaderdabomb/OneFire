using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingStateMachine : StateMachineBehaviour
{
    [SerializeField] private PlayerAnimation playerAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("ReelFish"))
        {
            FishCaughtProgressBar fishCaughtProgressBarInst = FishingManager.Instance.fishCaughtProgressBar.GetComponent<FishCaughtProgressBar>();
            bool onLastCatchSegment = fishCaughtProgressBarInst.OnLastSegment();
            animator.SetBool("onLastCatchSegment", onLastCatchSegment);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FishingCatchFish"))
        {
            FishingManager.Instance.StopFishing();
        }

        if (stateInfo.IsName("FishingCastHold"))
        {
            animator.SetBool("onLastCatchSegment", false);
            animator.ResetTrigger("canStartFishingTrigger");
            animator.ResetTrigger("canReelFishingTrigger");
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
