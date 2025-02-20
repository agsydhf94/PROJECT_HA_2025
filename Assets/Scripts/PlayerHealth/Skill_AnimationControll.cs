using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Skill_AnimationControll : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SkillManager.Instance.isSkillUsing = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SkillManager.Instance.isSkillUsing = false;
        }
    }
}
