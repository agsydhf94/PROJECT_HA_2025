using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CrossHair : MonoBehaviour
    {
        
        public Animator animator;

        private float gunAccuracy;
        public PlayerController PlayerController;

        [SerializeField]
        private GameObject crossHairHUD;

        private void Awake()
        {
            PlayerController = GameObject.Find("HA.Character.Player").GetComponent<PlayerController>();
            animator = GameObject.Find("CrossHair").GetComponent<Animator>();
        }

        private void Update()
        {
            WalkingAnimation(PlayerController.isWalk);            
            RunningAnimation(PlayerController.isSprint);            
            ClosedAimAnimation(PlayerController.isClosedAim);            
        }

        public void WalkingAnimation(bool flag)
        {
            animator.SetBool("Walking", flag);
        }
        
        public void RunningAnimation(bool flag)
        {
            animator.SetBool("Running", flag);
        }

        public void ClosedAimAnimation(bool flag)
        {
            animator.SetBool("ClosedAim", flag);
        }

        public void FireAnimation()
        {
            if(animator.GetBool("Walking"))
            {
                animator.SetTrigger("Walk_Fire");
            }
            
            if (animator.GetBool("ClosedAim"))
            {
                animator.SetTrigger("ClosedAim_Fire");
            }
        }
    }
}
