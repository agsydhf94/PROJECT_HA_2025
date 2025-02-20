using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{

    public class Projectile : MonoBehaviour
    {
        public float speed = 30f;
        public float lifeTime = 10;
        public float rifle_power = 20.0f;

        public GameObject metalImpactPrefab;
        public GameObject woodImpactPrefab;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {

            string physicMaterialName = collision.collider.material.name;
            string[] splitNames = physicMaterialName.Split(" ");
            string originalName = splitNames[0];

            if (originalName.Equals("PhysicMaterial_Metal"))
            {
                var newImpact = Instantiate(metalImpactPrefab);
                newImpact.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.Euler(collision.contacts[0].normal));
            }

            if (originalName.Equals("PhysicMaterial_Wood"))
            {
                var newImpact = Instantiate(woodImpactPrefab);
                newImpact.transform.SetPositionAndRotation(collision.contacts[0].point, Quaternion.Euler(-collision.contacts[0].normal));
            }

            Destroy(gameObject);


        }

        // 플레이어가 뚫을 수 있고 파괴가능한 오브젝트 데미지
        private void OnTriggerEnter(Collider other)
        {
            IDamagable damagable = other.GetComponent<IDamagable>();
            Debug.Log(other.name);
            if (damagable != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                damagable.Damage(rifle_power, hitPoint, hitNormal);
            }
        }


    }
}
