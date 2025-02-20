using HA;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Animations;
// using UnityEngine.Animations.Rigging;

public class AnimationRigController_RifleOff : StateMachineBehaviour
{
    public PlayerController playerController;

    private void Awake()
    {        
        // animator = GameObject.Find("FreeTestCharacterAsuna").GetComponent<Animator>();
        // playerController = GameObject.Find("HA.Character.Player").GetComponent<PlayerController>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = PlayerController.Instance;
        playerController.IsEnableMovemnt = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Rifle.SetActive(false);
        animator.SetBool("Rifle_Active", false);
        playerController.IsEnableMovemnt = true;
    }

}

