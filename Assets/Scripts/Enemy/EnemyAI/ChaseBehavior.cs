using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HA
{
    public class ChaseBehavior : StateMachineBehaviour
    {
        NavMeshAgent navMeshAgent;
        Transform playerCharacter;

        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent = animator.GetComponent<NavMeshAgent>();
            playerCharacter = GameObject.FindGameObjectWithTag("Player").transform;
        }

        
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent.SetDestination(playerCharacter.position);

            float distance = Vector3.Distance(playerCharacter.position, animator.transform.position);
            if (distance < 7)
            {
                animator.SetBool("isAttacking", true);
            }
        }

        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
        }

    }
}
