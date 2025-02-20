using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/WeaponSO")]
    public class WeaponSO : ScriptableObject
    {
        [Header("Weapon Category")]
        public WeaponCategory weaponCategory; // 무기 종류 (Rifle, Katana 등)

        [Header("Enemy Layer")]
        public LayerMask enemyLayer;

        [Header("Common Properties")]
        public string weaponName;          // 무기 이름
        public Sprite weaponSprite;        // 무기 아이콘 이미지
        public string weaponDescription;   // 무기 설명
        public int damagePoint;           // 기본 데미지

        [Header("Initial Bullet Setting")]
        public int initial_bullets_InMagazine;
        public int initial_bullets_Total;
        

        [Header("Shooting Weapon Properties")]
        public bool isSingleShot;        // 단발 여부
        public float fireRate;           // 발사 속도 (총기류에 해당)
        public int magazineCapacity;          // 탄창 용량
        public float shootingRaycastMaxRange;
        public GameObject bulletCasingPrefab;
        public GameObject muzzlePrefab;
        public ParticleSystem bulletImpactParticle;
        public TrailRenderer bulletTrailRenderer;
        public float bulletTrail_SpeedMultiplier;
        public AudioClip fire_Sound;
        public AudioClip reload_Sound;

        [Header("Missile Launcher Settings")]
        public GameObject missilePrefab;
        public float targetingRange = 100f; // 조준 범위
        public float aimRadius = 1.5f; // 조준원의 반경

        [Header("Missile Launch Initial Transform")]
        public Vector3 missileFire_Position;
        public Vector3 missileFire_Rotation;
        public Vector3 missileFire_Scale;
        

        [Header("Fire Transform Offset")]
        public Vector3 fireTransform_Position;
        public Vector3 fireTransform_Rotation;
        public Vector3 fireTransform_Scale;

        [Header("Bullet Casing Transform Offset")]
        public Vector3 bulletCasingTransform_Position;
        public Vector3 bulletImpactTransform_Rotation;
        public Vector3 bulletImpactTransform_Scale;

        [Header("Close Range Weapon Properties")]
        public float katanaRange;        // 휘두르는 범위 (칼이나 Katana)
        public GameObject swingVfx;      // 휘두를 때의 VFX 이펙트


        


        

    }
}
