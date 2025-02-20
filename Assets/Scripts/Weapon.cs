using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace HA
{
    public class Weapon : MonoBehaviourPun, IPunObservable
    {

        [Header("Weapon SO")]
        public WeaponSO weaponSO;

        [Header("Weapon Animation")]
        public Animator animator;       // 애니메이터 컴포넌트
        public AnimationClip reloadAnimationClip;  // 재장전 애니메이션을 직접 할당

        [Header("Common Properties")]
        public string weaponName;
        public Sprite weaponSprite;
        public string weaponDescription;
        public int damagePoint;





        [Header("Gun - SingleShot")]
        public bool canShoot = true; // 단발 모드에서 한 번만 발사하기 위한 플래그

        [Header("Gun - Ammo Information")]
        public int currentBullet_inMagazine; // 현재 탄창에 몇 발 남아있는지
        [SerializeField] protected int maxBulletCount; // 인벤토리에 저장 가능한 최대 탄환수
        public int bulletTotal; // 현재 소유중인 탄환 개수
       
        [Header("Gun - Recoil Pattern")]
        [SerializeField] protected List<Vector3> recoilShakePattern = new List<Vector3>();
        [SerializeField] protected int currentRecoilIndex = 0;
        [SerializeField] protected float retroActionForce; // 총기 반동세기
        [SerializeField] protected float retroActionFineSightForce; // 정조준 시 반동세기

        [Header("Gun - Shooting Values")]
        public bool isReload = false;
        [SerializeField] protected float lastShootTime = 0f;

        [Header("Katana - Attack Range")]
        public float range = 10f;

        public WeaponCache_TEMP WeaponCache
        {
            get
            {
                return WeaponData_CacheSaveAndExtracter();
            }
            set
            {
                WeaponData_CacheLoader(value);
            }
        }





        [Header("Audio")]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip reload_Sound;

        [Header("Miscellaneous")]
        [SerializeField] protected CrossHair crosshair;
        public GameObject firePosition;
        public GameObject metalImpactPrefab;
        public GameObject woodImpactPrefab;


                

        private void Start()
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.CompareTag("FirePosition"))
                {
                    firePosition = child.gameObject;
                    break; // 찾으면 반복 종료
                }
            }

            crosshair = FindObjectOfType<CrossHair>();
            audioSource = transform.root.GetComponent<AudioSource>();            
        }

        public void WeaponData_Initialize()
        {
            currentBullet_inMagazine = weaponSO.initial_bullets_InMagazine;
            bulletTotal = weaponSO.initial_bullets_Total;
        }

        public void WeaponData_CacheLoader(WeaponCache_TEMP weaponCache)
        {
            currentBullet_inMagazine = weaponCache.currentBullet_IN_MAGAZINE;
            bulletTotal = weaponCache.bulletTOTAL;
        }

        public WeaponCache_TEMP WeaponData_CacheSaveAndExtracter()
        {
            WeaponCache_TEMP weaponCache_Temp = new WeaponCache_TEMP();
            weaponCache_Temp.currentBullet_IN_MAGAZINE = currentBullet_inMagazine;
            weaponCache_Temp.bulletTOTAL = bulletTotal;

            return weaponCache_Temp;
        }

        public virtual void Attack()
        {

        }

        public void Reload()
        {
            if (bulletTotal > 0)
            {
                bulletTotal += currentBullet_inMagazine;
                currentBullet_inMagazine = 0;

                if (bulletTotal >= weaponSO.magazineCapacity) // 한 번 장전 가능한 충분한 탄을 소유하고 있다면
                {
                    currentBullet_inMagazine = weaponSO.magazineCapacity; // 정상 장전
                    bulletTotal -= weaponSO.magazineCapacity; // 장전한 탄 수만큼 소유분에서 차감
                }
                else // 장전 하는데 충분한 탄약이 없다면 ( ex-> 30발 장전해야 하는데 가진건 20발 뿐일 때 )
                {
                    currentBullet_inMagazine = bulletTotal;
                    bulletTotal = 0;
                }
            }
        }












        /*
        private IEnumerator DrawBulletTrail(Vector3 startPosition, Vector3 endPosition)
        {

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
            lineRenderer.material = lineMaterial;
            lineRenderer.enabled = true;
            Debug.Log("Start drawing bullet trail");

            yield return new WaitForSeconds(0.05f); // 0.05초 동안 라인을 보이게 함

            lineRenderer.enabled = false;
            Debug.Log("End drawing bullet trail");
        }
        */

        /*
        private void Shooting_Projectile()
        {
            // 총알 생성하기_Projectile
            var newBullet = Instantiate(bulletPrefab);
            var cameraTransform = Camera.main.transform;
            newBullet.transform.SetPositionAndRotation(weaponSO.fireTransform_Position, Quaternion.Euler(weaponSO.fireTransform_Rotation));
            newBullet.gameObject.SetActive(true);
        }
        */





        /*
        public IEnumerator ReloadCoroutine()
        {
            if (carryBulletCount > 0)
            {
                isReload = true;
                carryBulletCount += currentBulletCount;
                currentBulletCount = 0;

                yield return new WaitForSeconds(reloadTime);

                if (carryBulletCount >= reloadBulletCount) // 한 번 장전 가능한 충분한 탄을 소유하고 있다면
                {
                    currentBulletCount = reloadBulletCount; // 정상 장전
                    carryBulletCount -= reloadBulletCount; // 장전한 탄 수만큼 소유분에서 차감
                }
                else // 장전 하는데 충분한 탄약이 없다면 ( ex-> 30발 장전해야 하는데 가진건 20발 뿐일 때 )
                {
                    currentBulletCount = carryBulletCount;
                    carryBulletCount = 0;
                }

                isReload = false;
            }
        }
        */

        public void CancelReload()
        {
            if (isReload == true)
            {
                StopAllCoroutines();
                isReload = false;
            }
        }


        public void PlaySE(AudioClip _clip)
        {
            // audioSource.clip = _clip;
            audioSource.PlayOneShot(_clip);
        }

        // IPunObservable 인터페이스에 있는 함수로,
        // 클라이언트에서 리모트로 정보 동기화
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 로컬 오브젝트일 경우 실행
            // 현재 오브젝트가 로컬일 경우, 쓰기 모드가 됨
            if(stream.IsWriting)
            {
                stream.SendNext(bulletTotal);
                stream.SendNext(currentBullet_inMagazine);
            }
            else // 리모트 오브젝트일 경우 실행
            {
                bulletTotal = (int)stream.ReceiveNext();
                currentBullet_inMagazine = (int)stream.ReceiveNext();
            }
        }

        

        [PunRPC]
        public void AddBullets(int ammo)
        {
            bulletTotal += ammo;
        }





        // RPG 용
        public void DetectAndDealDamage(int addDamagePoint)
        {

            int enemyLayerMask = LayerMask.GetMask("Enemy");

            // 공격 범위 내의 모든 적을 감지
            Collider[] hit = Physics.OverlapSphere(transform.position, weaponSO.katanaRange, enemyLayerMask);

            foreach (Collider enemy in hit)
            {
                // 적의 Health 컴포넌트 가져오기
                IDamagable damagable = enemy.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    // hitPoint: 플레이어와 적 콜라이더 사이의 가장 가까운 지점
                    Vector3 hitPoint = enemy.ClosestPoint(transform.position);

                    // hitNormal: 플레이어에서 적으로 향하는 방향 벡터
                    Vector3 hitNormal = (enemy.transform.position - transform.position).normalized;

                    int finalDamagePoint = weaponSO.damagePoint + addDamagePoint;

                    // 데미지 주기
                    damagable.Damage(finalDamagePoint, hitPoint, hitNormal);

                }
            }
        }

        





    }

    public enum WeaponCategory
    {
        Rifle,
        Pistol,
        MissileLauncher,
        Katana,
        Bomb
    }

    public struct WeaponCache_TEMP
    {
        public int currentBullet_IN_MAGAZINE;
        public int bulletTOTAL;
    }
}
