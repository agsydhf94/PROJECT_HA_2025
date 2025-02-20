using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public string weaponName;
        public int ammoCount;  // 현재 탄창에 있는 탄약 수
        public int MaxAmmoCount; // 탄창 용량

        public GameObject bulletPrefab;
        public Transform firePoint;

        public AudioClip shootSound;
        public ParticleSystem muzzleFlash;
        public WeaponCategory category;
        public IWeaponBehavior weaponBehavior;



        // 무기 프리팹을 생성할 때 유니티 에디터에서 각 무기의 카테고리를 설정하도록 하여 코드에서 하드코딩을 피함
        // 이를 위해,
        // 무기 카테고리를 유니티 인스펙터에서 설정 가능하도록
        // WeaponBase 클래스에 SerializeField를 추가하고, Gun 클래스에서 카테고리를 설정하지 않도록 수정

        [SerializeField]
        private WeaponCategory wCategory;
        
        public WeaponCategory WCategory => wCategory;

        public abstract void Shoot();

    }
}
