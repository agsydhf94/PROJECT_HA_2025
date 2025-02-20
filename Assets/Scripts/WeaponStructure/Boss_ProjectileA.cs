using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace HA
{
    public class Boss_ProjectileA : MonoBehaviour
    {
        [Header("Projectile Properties")]
        public float damagePoint;
        [SerializeField] private Vector3 attackDirection;
        public float projectileSpeed;

        public float timer;
        public float projectile_TimeLimit;
        public bool impactFlag;
        LayerMask layerMask;

        private void Start()
        {
            layerMask = LayerMask.GetMask("Character", "Enemy");
            impactFlag = false;
        }

        private void Update()
        {
            transform.position += attackDirection * projectileSpeed * Time.deltaTime;

        }

        public void SetTargetDirection(Vector3 direction, float projectileSpeed)
        {
            attackDirection = direction.normalized;
            this.projectileSpeed = projectileSpeed;
        }


        private async void OnTriggerEnter(Collider collision)
        {
            if (impactFlag) return;

            


            if ((layerMask & (1 << collision.gameObject.layer)) == 0 && !impactFlag)
            {
                Debug.Log($"충돌한 오브젝트 레이어: {collision.gameObject.layer}");

                impactFlag = true;
                var impactPrefab = ObjectPool.Instance.GetFromPool<ParticleSystem>("BossSegmentExplosionPrefab");
                impactPrefab.Play();
                impactPrefab.transform.position = collision.transform.position;
                await ImpactPrefab_Return(impactPrefab);

            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Character") && !impactFlag)
            {
                Debug.Log($"충돌한 오브젝트 레이어: {collision.gameObject.layer}");
                impactFlag = true;
                if (collision.transform.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.Damage(damagePoint, transform.position, transform.position);
                    var impactPrefab = ObjectPool.Instance.GetFromPool<ParticleSystem>("BossSegmentExplosionPrefab");
                    impactPrefab.Play();
                    impactPrefab.transform.position = collision.transform.position;
                    await ImpactPrefab_Return(impactPrefab);

                }
                /*
                else
                {
                    var impactPrefab = ObjectPool.Instance.GetFromPool<ParticleSystem>("BossSegmentExplosionPrefab");
                    impactPrefab.transform.position = collision.transform.position;
                    await ImpactPrefab_Return(impactPrefab);

                    
                }
                */
            }
            
            

        }


        private async UniTask ImpactPrefab_Return(ParticleSystem impactPrefab)
        {
            await UniTask.Delay(2000);
            ObjectPool.Instance.ReturnToPool("BossSegmentExplosionPrefab", impactPrefab);
        }
    }
}
