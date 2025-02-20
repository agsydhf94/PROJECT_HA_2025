using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletShootBehavior : IWeaponBehavior

{
    private float lastShootTime = 0f;
    public float fireRate = 0.1f;

    public void Attack(WeaponBase weapon)
    {
        
    }
}
