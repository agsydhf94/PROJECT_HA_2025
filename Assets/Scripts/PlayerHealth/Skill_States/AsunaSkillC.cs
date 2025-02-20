using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class AsunaSkillC : MonoBehaviour, ISkillState
    {
        public SkillSO skillSO;
        public float vfxStartDelay;

        public void ExecuteSkill()
        {
            StartCoroutine(SKillSequence());
        }

        public IEnumerator SKillSequence()
        {
            yield return new WaitForSeconds(vfxStartDelay);
            var vfxEffect1 = ParticleEffect_PlayEffect(1);

            yield return new WaitForSecondsRealtime(1.02f);
            var vfxEffect2 = ParticleEffect_PlayEffect(2);
            ParticleEffect_AfterTouch(vfxEffect1);
            ParticleEffect_AfterTouch(vfxEffect2);
            DetectEnemy_ByCylinder();

            yield return new WaitForSecondsRealtime(0.9f);
            ObjectPool.Instance.ReturnToPool(skillSO.skillName + "1", vfxEffect1);
            ObjectPool.Instance.ReturnToPool(skillSO.skillName + "2", vfxEffect2);


        }

        public void DetectEnemy_ByCylinder()
        {
            Vector3 startPosition = GameObject.FindGameObjectWithTag("Supershotgun_CylinderStart").transform.position;
            Vector3 endPosition = GameObject.FindGameObjectWithTag("Supershotgun_CylinderEnd").transform.position;
            Collider[] hitColliders = Physics.OverlapCapsule(startPosition, endPosition, skillSO.attackRange, skillSO.enemyLayer);

            foreach(var detectedEnemy in hitColliders)
            {
                IDamagable damagable = detectedEnemy.GetComponent<IDamagable>();
                damagable.Damage(skillSO.attackPoint, detectedEnemy.transform.position, -detectedEnemy.transform.position);
            }
        }

        public ParticleSystem ParticleEffect_PlayEffect(int particle_Num)
        {
            Transform parentTransform = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentWeapon.firePosition.transform;
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

            vfxEffect.transform.SetParent(null, true); // true: 부모의 월드 좌표계 유지
            vfxEffect.transform.rotation = Quaternion.Euler(-90f, -23.4f, 116.8f);
        }
        
    }
}
