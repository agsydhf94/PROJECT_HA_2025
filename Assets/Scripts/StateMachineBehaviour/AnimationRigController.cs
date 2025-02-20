using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
// using UnityEngine.Animations.Rigging;

public class AnimationRigController : StateMachineBehaviour
{  

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Rifle_Active", true);
        PlayerController.Instance.IsEnableMovemnt = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        animator.SetTrigger("Ready_Rifle");
        PlayerController.Instance.IsEnableMovemnt = true;
    }
    
}



