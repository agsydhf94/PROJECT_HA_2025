using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Gun : WeaponBase
    {
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float fireForce = 20f;

        private float lastShootTime = 0f;
        public float fireRate = 0.1f;

        public override void Shoot()
        {
            
        }
    }
}
