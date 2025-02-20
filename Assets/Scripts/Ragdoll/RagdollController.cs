using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HA
{
    public class RagdollController : MonoBehaviour
    {
        public Rigidbody[] rigidbodies;
        public Collider[] colliders;
        public Animator animator;
        public NavMeshAgent navMeshAgent;

        [Header("Ragdoll Foece Active")]
        public HumanBodyBones targetBone;
        public Vector3 forceDirection;
        public float forcePower;

        private void Awake()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            // SetRagdollActive(false);
            RidigBodyActive(false);
            ColliderActive(true);
        }

        [ContextMenu("Active Ragdoll With Power")]
        public void ActiveForceRagdollWithPower()       // 에디터에서 임시로 확인용 menu 함수
        {
            // public에 선언 된 임시 변수들을 활용하여 Ragdoll에 Force를 줘서 활성화 시킨다
            ForceActiveRagdollWithPower(targetBone, forceDirection, forcePower);
        }

        public void ForceActiveRagdollWithPower(HumanBodyBones bone, Vector3 direction, float power)
        {
            SetRagdollActive(true);

            var boneTransform = animator.GetBoneTransform(bone);            // Animator를 통해서, HumanBodyBones의 Transform을 가져옴
            var targetRigidbody = boneTransform.GetComponent<Rigidbody>();  // 가져온 Bone Transform에서 Rigidbody 가져옴
            targetRigidbody.AddForce(direction * power, ForceMode.Force);   // 가져온 Rigidbody에 힘을 가한다
        }

        // 디버깅 용
        [ContextMenu("Active Ragdoll")]
        public void ActiveRagdoll()
        {
            // to do : Ragdoll을 활성화 시킬때는 Animator, NavMesh 등을 모두 꺼주는 것이 좋음
            animator.enabled = false;
            navMeshAgent.enabled = false;
            SetRagdollActive(true);
        }

        public void SetRagdollActive(bool isActive)
        {
            RidigBodyActive(isActive);

            ColliderActive(isActive);
        }

        public void RidigBodyActive(bool isActive)
        {
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = !isActive;
            }
        }

        public void ColliderActive(bool isActive)
        {
            foreach (var col in colliders)
            {
                col.enabled = isActive;
            }
        }
    }
}
