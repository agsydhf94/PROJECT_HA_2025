using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CharacterAnimationController : MonoBehaviour
    {
        public Animator animator;
        public bool isRootMotionOn;


        private void OnAnimatorMove()
        {
            /*
            Vector3 newPosition = new Vector3(0, 0, 0);
            animator.transform.position = newPosition;
            Debug.Log(animator.gameObject.transform.position);
            */

//            Vector3 newPosition = Vector3.zero;
//            newPosition.x = 0;
//            newPosition.y = animator.transform.position.y;
//            newPosition.z = 0;

////
//            if (isRootMotionOn)
//            {
                  animator.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
//            }






            /*
            // 필요한 경우 부모의 위치를 Root Motion에 맞게 업데이트
            // 부모 위치 변경을 원하지 않는다면 아래 코드 삭제 가능
            Vector3 deltaPosition = animator.deltaPosition;
            Quaternion deltaRotation = animator.deltaRotation;

            transform.position += deltaPosition;
            transform.rotation *= deltaRotation;
            */
        }
    }
}
