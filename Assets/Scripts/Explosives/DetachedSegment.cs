using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class DetachedSegment : MonoBehaviour
    {
        public int key;
        public bool callBackFlag;
        public AudioSource audioSource;

        private void OnCollisionEnter(Collision collision)
        {
            // ���� �������� ���� ���� ����
            if(collision.gameObject.layer != LayerMask.NameToLayer("Enemy") && !callBackFlag)
            {
                BossEnemy.destroySequence_Grounded.Invoke((transform, key));
                callBackFlag = true;
            }
        }
       
    }
}
