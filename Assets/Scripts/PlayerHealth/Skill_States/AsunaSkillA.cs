using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace HA
{
    public class AsunaSkillA : MonoBehaviour, ISkillState
    {
        public SkillSO skillSO;
        public float vfxStartDelay;
        

        public void ExecuteSkill()
        {
            StartCoroutine(SkillSequence());
        }


        public IEnumerator SkillSequence()
        {
            yield return new WaitForSeconds(vfxStartDelay);
            var vfxEffect = ParticleEffect_PlayEffect();
            
            yield return new WaitForSeconds(1.66f - vfxStartDelay);
            ParticleEffect_AfterTouch(vfxEffect);
            DetectEnemy_BySector();

            yield return new WaitForSeconds(0.5f);
            ObjectPool.Instance.ReturnToPool(skillSO.skillName, vfxEffect);
        }



        public void DetectEnemy_BySector()
        {
            var playerTransform = GameManager.Instance.currentActiveCharacter.transform;
            Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, skillSO.attackRange, skillSO.enemyLayer);


            foreach(var detectedEnemy in  hitColliders)
            {
                Vector3 directionToEnemy = (detectedEnemy.transform.position - playerTransform.position).normalized;
                float angleToEnemy = Vector3.Angle(directionToEnemy, playerTransform.forward);
                
                if(angleToEnemy < skillSO.skillHalfAngle)
                {
                    IDamagable damagable = detectedEnemy.GetComponent<IDamagable>();
                    damagable.Damage(skillSO.attackPoint, detectedEnemy.transform.position, directionToEnemy);
                }
                
            }
        }

        public ParticleSystem ParticleEffect_PlayEffect()
        {
            Transform parentTransform = GameObject.FindGameObjectWithTag("ParentTransform_Hand_L").transform;
            var vfxEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>(skillSO.skillName);
            vfxEffect.gameObject.transform.SetParent(parentTransform);
            vfxEffect.transform.localPosition = Vector3.zero;
            vfxEffect.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            vfxEffect.transform.localScale = Vector3.one;

            if (vfxEffect != null)
            {
                vfxEffect.Play();
            }

            return vfxEffect;
        }

        public void ParticleEffect_AfterTouch(ParticleSystem vfxEffect)
        {
            Quaternion vfxEffect_WorldRotation_Q = vfxEffect.transform.parent.rotation * vfxEffect.transform.localRotation;
            Vector3 vfxEffect_WorldRotation_V = vfxEffect_WorldRotation_Q.eulerAngles;
            Vector3 vfxEffect_WorldForwardDirection = vfxEffect.transform.forward;

            vfxEffect.transform.SetParent(null, true); // true: 부모의 월드 좌표계 유지
            vfxEffect.transform.forward = vfxEffect_WorldForwardDirection;
            vfxEffect.transform.rotation = Quaternion.Euler(0, vfxEffect_WorldRotation_V.y, 90);            
        }



        public void ParticleEffect_PlaySound()
        {

        }
    }
}
