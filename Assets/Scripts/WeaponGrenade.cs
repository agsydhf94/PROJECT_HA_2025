using System.Collections;
using UnityEngine;

namespace HA
{
    public class WeaponGrenade : MonoBehaviour
    {
        public GameObject explosionEffectPrefab;  // Prefab ��� Object Pooling���� �������� ��ü ���
        public MeshRenderer meshRenderer;
        public float explosionRadius = 13.0f;
        public float explosionForce = 400.0f;
        public float throwForce = 500.0f;

        public float explosionDamage;
        public new Rigidbody rigidbody;

        public void Settings(float damage, Vector3 rotation)
        {
            rigidbody = gameObject.GetComponent<Rigidbody>();
            rigidbody.AddForce(rotation * throwForce);

            explosionDamage = damage;
        }

        private void Start()
        {
            StartCoroutine(Explosion());
        }

        public IEnumerator Explosion()
        {
            yield return new WaitForSeconds(3f);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            ParticleSystem explosionEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("GrenadeExplosionEffect");
            explosionEffect.transform.position = transform.position;
            explosionEffect.transform.rotation = transform.rotation;
            explosionEffect.gameObject.SetActive(true);

            if (!explosionEffect.isPlaying) // ����� ��
                explosionEffect.Play();

            // ��ƼŬ�� ���� ������ ���
            yield return new WaitUntil(() => !explosionEffect.isPlaying);



            meshRenderer.enabled = false;

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

            foreach (RaycastHit raycastHit in rayHits)
            {
                raycastHit.transform.GetComponent<Enemy>().DamageByGrenade(explosionDamage, transform.position, explosionForce);
            }

            ObjectPool.Instance.ReturnToPool("GrenadeExplosionEffect", explosionEffect);

        }
    }
}
