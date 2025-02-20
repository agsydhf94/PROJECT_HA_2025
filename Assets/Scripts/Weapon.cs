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
        public Animator animator;       // �ִϸ����� ������Ʈ
        public AnimationClip reloadAnimationClip;  // ������ �ִϸ��̼��� ���� �Ҵ�

        [Header("Common Properties")]
        public string weaponName;
        public Sprite weaponSprite;
        public string weaponDescription;
        public int damagePoint;





        [Header("Gun - SingleShot")]
        public bool canShoot = true; // �ܹ� ��忡�� �� ���� �߻��ϱ� ���� �÷���

        [Header("Gun - Ammo Information")]
        public int currentBullet_inMagazine; // ���� źâ�� �� �� �����ִ���
        [SerializeField] protected int maxBulletCount; // �κ��丮�� ���� ������ �ִ� źȯ��
        public int bulletTotal; // ���� �������� źȯ ����
       
        [Header("Gun - Recoil Pattern")]
        [SerializeField] protected List<Vector3> recoilShakePattern = new List<Vector3>();
        [SerializeField] protected int currentRecoilIndex = 0;
        [SerializeField] protected float retroActionForce; // �ѱ� �ݵ�����
        [SerializeField] protected float retroActionFineSightForce; // ������ �� �ݵ�����

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
                    break; // ã���� �ݺ� ����
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

                if (bulletTotal >= weaponSO.magazineCapacity) // �� �� ���� ������ ����� ź�� �����ϰ� �ִٸ�
                {
                    currentBullet_inMagazine = weaponSO.magazineCapacity; // ���� ����
                    bulletTotal -= weaponSO.magazineCapacity; // ������ ź ����ŭ �����п��� ����
                }
                else // ���� �ϴµ� ����� ź���� ���ٸ� ( ex-> 30�� �����ؾ� �ϴµ� ������ 20�� ���� �� )
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

            yield return new WaitForSeconds(0.05f); // 0.05�� ���� ������ ���̰� ��

            lineRenderer.enabled = false;
            Debug.Log("End drawing bullet trail");
        }
        */

        /*
        private void Shooting_Projectile()
        {
            // �Ѿ� �����ϱ�_Projectile
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

                if (carryBulletCount >= reloadBulletCount) // �� �� ���� ������ ����� ź�� �����ϰ� �ִٸ�
                {
                    currentBulletCount = reloadBulletCount; // ���� ����
                    carryBulletCount -= reloadBulletCount; // ������ ź ����ŭ �����п��� ����
                }
                else // ���� �ϴµ� ����� ź���� ���ٸ� ( ex-> 30�� �����ؾ� �ϴµ� ������ 20�� ���� �� )
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

        // IPunObservable �������̽��� �ִ� �Լ���,
        // Ŭ���̾�Ʈ���� ����Ʈ�� ���� ����ȭ
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // ���� ������Ʈ�� ��� ����
            // ���� ������Ʈ�� ������ ���, ���� ��尡 ��
            if(stream.IsWriting)
            {
                stream.SendNext(bulletTotal);
                stream.SendNext(currentBullet_inMagazine);
            }
            else // ����Ʈ ������Ʈ�� ��� ����
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





        // RPG ��
        public void DetectAndDealDamage(int addDamagePoint)
        {

            int enemyLayerMask = LayerMask.GetMask("Enemy");

            // ���� ���� ���� ��� ���� ����
            Collider[] hit = Physics.OverlapSphere(transform.position, weaponSO.katanaRange, enemyLayerMask);

            foreach (Collider enemy in hit)
            {
                // ���� Health ������Ʈ ��������
                IDamagable damagable = enemy.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    // hitPoint: �÷��̾�� �� �ݶ��̴� ������ ���� ����� ����
                    Vector3 hitPoint = enemy.ClosestPoint(transform.position);

                    // hitNormal: �÷��̾�� ������ ���ϴ� ���� ����
                    Vector3 hitNormal = (enemy.transform.position - transform.position).normalized;

                    int finalDamagePoint = weaponSO.damagePoint + addDamagePoint;

                    // ������ �ֱ�
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
