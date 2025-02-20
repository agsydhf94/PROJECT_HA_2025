using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ReloadBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerController.Instance.ReloadFinished();
        }
    }
}

