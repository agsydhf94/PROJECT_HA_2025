using System.Collections;
using UnityEngine;

namespace HA
{
    public class AsunaSkillB : MonoBehaviour, ISkillState
    {
        public SkillSO skillSO;
        public Transform initialTransform;
        public float vfxStartDelay1;
        public float vfxStartDelay2;
        public float vfxStartDelay3;

        public void ExecuteSkill()
        {
            StartCoroutine(SkillSequence());
        }

        public IEnumerator SkillSequence()
        {
            yield return new WaitForSecondsRealtime(vfxStartDelay1 - 0.2f) ;
            var vfxEffect1 = ParticleEffect_PlayEffect(1);
            yield return new WaitForSecondsRealtime(0.62f);
            ParticleEffect_AfterTouch(vfxEffect1);
            DetectEnemy_BySphere(vfxEffect1);


            yield return new WaitForSecondsRealtime(vfxStartDelay2 - 0.2f);
            var vfxEffect2 = ParticleEffect_PlayEffect(2);
            yield return new WaitForSecondsRealtime(0.62f);
            ObjectPool.Instance.ReturnToPool(skillSO.skillName + "1", vfxEffect1);
            ParticleEffect_AfterTouch(vfxEffect2);
            DetectEnemy_BySphere(vfxEffect2);


            yield return new WaitForSecondsRealtime(vfxStartDelay3 - 0.2f);
            var vfxEffect3 = ParticleEffect_PlayEffect(3);
            yield return new WaitForSecondsRealtime(0.62f);
            ObjectPool.Instance.ReturnToPool(skillSO.skillName + "2", vfxEffect2);
            ParticleEffect_AfterTouch(vfxEffect3);
            DetectEnemy_BySphere(vfxEffect3);

            yield return new WaitForSecondsRealtime(0.8f);                        
            ObjectPool.Instance.ReturnToPool(skillSO.skillName + "3", vfxEffect3);
        }

        public void DetectEnemy_BySphere(ParticleSystem vfxEffect)
        {

            Transform burstCenter_Transfrom = TargetChild(vfxEffect.transform, "EnergyPulseHitPoint");
            Collider[] colliders = Physics.OverlapSphere(burstCenter_Transfrom.position, skillSO.attackRange, skillSO.enemyLayer);

            foreach(var detectedEnemy in colliders)
            {
                if(IsTargetVisible(burstCenter_Transfrom.position, detectedEnemy.transform))
                {
                    IDamagable damagable = detectedEnemy.GetComponent<IDamagable>();
                    damagable.Damage(skillSO.attackPoint, detectedEnemy.transform.position, -detectedEnemy.transform.forward);
                }
            }
        }

        public ParticleSystem ParticleEffect_PlayEffect(int particle_Num)
        {
            Transform parentTransform = GameObject.FindGameObjectWithTag("ParentTransform_Hand_L").transform;
            var vfxEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>(skillSO.skillName + $"{particle_Num}");
            vfxEffect.gameObject.transform.SetParent(parentTransform);
            vfxEffect.transform.localPosition = Vector3.zero;
            vfxEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            vfxEffect.transform.localScale = Vector3.one;

            if (vfxEffect != null)
            {
                vfxEffect.Play();
            }

            return vfxEffect;
        }

        public void ParticleEffect_AfterTouch(ParticleSystem vfxEffect)
        {
            Vector3 vfxEffect_WorldForwardDirection = SkillManager.Instance.inintialTransform.forward;

            vfxEffect.transform.SetParent(null, true); // true: 부모의 월드 좌표계 유지
            vfxEffect.transform.forward = vfxEffect_WorldForwardDirection;
            vfxEffect.transform.rotation = Quaternion.LookRotation(vfxEffect_WorldForwardDirection)
                                           * Quaternion.Euler(new Vector3(90f, 0f, 180f));            
        }

        public Transform TargetChild(Transform parentTransform, string tag)
        {
            foreach(Transform child in parentTransform.transform)
            {
                if(child.CompareTag(tag))
                {
                    return child;
                }                
            }
            return null;
        }

        private bool IsTargetVisible(Vector3 explosionCenter, Transform target)
        {
            // 대상의 콜라이더 중심을 향한 방향 계산
            Vector3 directionToTarget = target.position - explosionCenter;

            // Raycast를 사용해 가시선 체크
            if (Physics.Raycast(explosionCenter, directionToTarget.normalized, out RaycastHit hit, skillSO.attackRange, skillSO.enemyLayer))
            {
                // 가시선이 적 오브젝트에 먼저 닿은 경우만 true 반환
                if (hit.transform == target)
                {
                    return true;
                }
            }

            return false; // 장애물로 막혀 있으면 false
        }

    }
}
