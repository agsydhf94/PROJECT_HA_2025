using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class idleBehavior : StateMachineBehaviour
    {

        float timer;

        Transform playerCharacter;
        float chaseRange = 10.0f;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer = 0;
            playerCharacter = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer += Time.deltaTime;
            if (timer > 5)
            {
                animator.SetBool("isPatrolling", true);
            }

            float distance = Vector3.Distance(playerCharacter.position, animator.transform.position);
            if (distance < chaseRange)
            {
                animator.SetBool("isChasing", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }


    }
}
