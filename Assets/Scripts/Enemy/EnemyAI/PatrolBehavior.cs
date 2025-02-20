using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HA
{
    public class PatrolBehavior : StateMachineBehaviour
    {

        float timer = 0;
        List<Transform> wayPoints = new List<Transform>();
        NavMeshAgent navMeshAgent;
        private int lastSelectedIndex = -1;


        Transform playerCharacter;
        float chaseRange = 10.0f;


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer = 0;
            GameObject[] wayPointsObject = GameObject.FindGameObjectsWithTag("EnemyWaypoints");
            foreach (GameObject wayPointElements in wayPointsObject)
            {
                wayPoints.Add(wayPointElements.transform);
            }
            navMeshAgent = animator.GetComponent<NavMeshAgent>();

            playerCharacter = GameObject.FindGameObjectWithTag("Player").transform;
        }


        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                int randomIndex = Random.Range(0, wayPoints.Count);
                while (randomIndex == lastSelectedIndex)
                {
                    randomIndex = Random.Range(0, wayPoints.Count);
                }
                lastSelectedIndex = randomIndex;
                navMeshAgent.SetDestination(wayPoints[randomIndex].position);
            }


            timer += Time.deltaTime;
            if (timer > 10)
            {
                animator.SetBool("isPatrolling", false);               
            }


            float distance = Vector3.Distance(playerCharacter.position, animator.transform.position);
            if(distance < chaseRange)
            {
                animator.SetBool("isChasing", true);
            }

        }

        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
        }


    }
}
