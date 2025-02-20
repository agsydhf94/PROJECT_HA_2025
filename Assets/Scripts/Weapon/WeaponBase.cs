using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public string weaponName;
        public int ammoCount;  // ���� źâ�� �ִ� ź�� ��
        public int MaxAmmoCount; // źâ �뷮

        public GameObject bulletPrefab;
        public Transform firePoint;

        public AudioClip shootSound;
        public ParticleSystem muzzleFlash;
        public WeaponCategory category;
        public IWeaponBehavior weaponBehavior;



        // ���� �������� ������ �� ����Ƽ �����Ϳ��� �� ������ ī�װ��� �����ϵ��� �Ͽ� �ڵ忡�� �ϵ��ڵ��� ����
        // �̸� ����,
        // ���� ī�װ��� ����Ƽ �ν����Ϳ��� ���� �����ϵ���
        // WeaponBase Ŭ������ SerializeField�� �߰��ϰ�, Gun Ŭ�������� ī�װ��� �������� �ʵ��� ����

        [SerializeField]
        private WeaponCategory wCategory;
        
        public WeaponCategory WCategory => wCategory;

        public abstract void Shoot();

    }
}
