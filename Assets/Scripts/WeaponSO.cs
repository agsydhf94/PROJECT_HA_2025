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
        public WeaponCategory weaponCategory; // ���� ���� (Rifle, Katana ��)

        [Header("Enemy Layer")]
        public LayerMask enemyLayer;

        [Header("Common Properties")]
        public string weaponName;          // ���� �̸�
        public Sprite weaponSprite;        // ���� ������ �̹���
        public string weaponDescription;   // ���� ����
        public int damagePoint;           // �⺻ ������

        [Header("Initial Bullet Setting")]
        public int initial_bullets_InMagazine;
        public int initial_bullets_Total;
        

        [Header("Shooting Weapon Properties")]
        public bool isSingleShot;        // �ܹ� ����
        public float fireRate;           // �߻� �ӵ� (�ѱ���� �ش�)
        public int magazineCapacity;          // źâ �뷮
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
        public float targetingRange = 100f; // ���� ����
        public float aimRadius = 1.5f; // ���ؿ��� �ݰ�

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
        public float katanaRange;        // �ֵθ��� ���� (Į�̳� Katana)
        public GameObject swingVfx;      // �ֵθ� ���� VFX ����Ʈ


        


        

    }
}
