using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class WeaponSO_DTO : MonoBehaviour
    {
        public LayerMask enemyLayer;

        public string weaponName;
        public string weaponCategory;
        public string weaponDescription;
        public string weaponSpritePath;
        public int damagePoint;

        // 초기 총알 수량
        public int initial_bullets_InMagazine;
        public int initial_bullets_Total;

        // 사격 무기 속성
        public bool isSingleShot;
        public float fireRate;
        public int magazineCapacity;
        public float shootingRaycastMaxRange;
        public GameObject bulletCasingPrefab;
        public GameObject muzzlePrefab;
        public ParticleSystem bulletImpactParticle;
        public TrailRenderer bulletTrailRenderer;
        public float bulletTrail_SpeedMultiplier;
        public AudioClip fire_Sound;
        public AudioClip reload_Sound;

        // 미사일 발사기 설정
        public GameObject missilePrefab;
        public float targetingRange;
        public float aimRadius;

        // 미사일 발사 초기 Transform (각 성분 개별 저장)
        public float missileFire_PositionX;
        public float missileFire_PositionY;
        public float missileFire_PositionZ;
        public float missileFire_RotationX;
        public float missileFire_RotationY;
        public float missileFire_RotationZ;
        public float missileFire_ScaleX;
        public float missileFire_ScaleY;
        public float missileFire_ScaleZ;

        // 사격 위치 오프셋 (각 성분 개별 저장)
        public float fireTransform_PositionX;
        public float fireTransform_PositionY;
        public float fireTransform_PositionZ;
        public float fireTransform_RotationX;
        public float fireTransform_RotationY;
        public float fireTransform_RotationZ;
        public float fireTransform_ScaleX;
        public float fireTransform_ScaleY;
        public float fireTransform_ScaleZ;

        // 탄피 위치 오프셋 (각 성분 개별 저장)
        public float bulletCasingTransform_PositionX;
        public float bulletCasingTransform_PositionY;
        public float bulletCasingTransform_PositionZ;
        public float bulletImpactTransform_RotationX;
        public float bulletImpactTransform_RotationY;
        public float bulletImpactTransform_RotationZ;
        public float bulletImpactTransform_ScaleX;
        public float bulletImpactTransform_ScaleY;
        public float bulletImpactTransform_ScaleZ;

        // 근접 무기 속성
        public float katanaRange;
        public string swingVfxPath;
    }
}
