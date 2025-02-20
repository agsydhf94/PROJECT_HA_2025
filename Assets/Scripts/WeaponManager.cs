using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace HA
{
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager instance;

        public static WeaponManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WeaponManager>();
                }
                return instance;
            }


        }

        


        [Header("UI Prefabs")]
        public MissileTargetUI targetIndicator;

        [Header("GameObject Prefabs")]
        public Missile_Projectile missilePrefab;

        [Header("Explosion Prefab")]
        public ParticleSystem explosionPrefab;
        public ParticleSystem bossSegmentexplosionPrefab;

        [Header("Projectile Prefabs")]
        public VisualEffect boss_ProjectileA;
        public VisualEffect boss_ProjectileB;
        public VisualEffect boss_ProjectileC;

        [Header("Boss Warning Circle Prefab")]
        public ParticleSystem bossWarningCircle;

        [Header("Dummy Object Prefabs")]
        public DetachedSegment bossEnemySegmenmt_Dummy;

        
        [Header("MissileImpactParticle Prefab")]
        public ParticleSystem missileImpactParticle;

        [Header("Grenade Properties")]
        public GameObject grenadePrefab;
        public GameObject grenade_Dummy;
        public int grenadeCount;
        public ParticleSystem grenadeExplosionEffect;

        // ���� ���� ����� �����ϰ� �ִ� Ÿ��
        // ���� ��ü ��ũ��Ʈ���� ���� �� WeaponManager�� ���� ����
        // �̻��� źȯ�� ������ ��ũ��Ʈ���� ������ ���޵Ǿ� ź�� ������
        [Header("Current Aiming Target")]
        public TargetToStrike targetToStrike;


        private int index = 0;
        // private bool isSwitching = false;



        private void Start()
        {
            // Effects
            ObjectPool.Instance.CreatePool("GrenadeExplosionEffect", grenadeExplosionEffect, 10);
            ObjectPool.Instance.CreatePool("MissileImpactParticle", missileImpactParticle, 5);
            ObjectPool.Instance.CreatePool("ExplosionPrefab", explosionPrefab, 10);
            ObjectPool.Instance.CreatePool("BossSegmentExplosionPrefab", bossSegmentexplosionPrefab, 10);
            ObjectPool.Instance.CreatePool("bossWarningCircle", bossWarningCircle, 10);

            // Boss Projectiles
            ObjectPool.Instance.CreatePool("boss_ProjectileA", boss_ProjectileA, 5);
            ObjectPool.Instance.CreatePool("boss_ProjectileB", boss_ProjectileB, 5);
            ObjectPool.Instance.CreatePool("boss_ProjectileC", boss_ProjectileC, 5);

            ObjectPool.Instance.CreatePool("MissilePrefab", missilePrefab, 5);
            ObjectPool.Instance.CreatePool("bossEnemySegmenmt_Dummy", bossEnemySegmenmt_Dummy, 8);
            ObjectPool.Instance.CreatePool("MissileTargetUI", targetIndicator, 3);
        }




        public void GrenadeThrow(string eventName)
        {
            if (eventName.Equals("Grenade_Throw") && grenadeCount > 0)
            {
                Vector3 throwPosition = grenade_Dummy.transform.position;
                grenade_Dummy.SetActive(false);

                GameObject grenade = Instantiate(grenadePrefab, throwPosition, grenade_Dummy.transform.rotation);

                Rigidbody rigidbody = grenade.GetComponent<Rigidbody>();
                rigidbody.AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);

                grenadeCount--;                
            }
        }

        public T GetCurrentWeapon<T>() where T : Weapon
        {
            return GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentWeapon as T;
        }

        public void GetDummySegment_FromPool()
        {
            // ���� ����Ʈ�� ������Ʈ Ǯ���� ������
            var explosionEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("ExplosionPrefab");
            explosionEffect.transform.position = transform.position; 
        }

        





        // ĳ������ ���� �迭�� �����ϴ� �Լ�

        /*
        public void SetCharacterWeapons(CharacterMode characterMode)
        {
            if (characterMode == CharacterMode.Shooter)
            {
                weapon = weapon_TPS;
            }
            else if (characterMode == CharacterMode.Kunoichi)
            {
                weapon = weapon_RPG;
            }

            
        }

        // ���� �Ѵ� ����� �ȵ�
        // equippen �� ������ ���� ������ҿ� ���� ������ �ʿ���
        
        public void InitializeWeapon()
        {
            for (int i = 0; i < weapon.Length; i++)
            {
                weapon[i].SetActive(false);
            }
            weapon[0].SetActive(true);
            index = 0;
        }

        private IEnumerator SwitchDelay(int newIndex)
        {
            isSwitching = true;
            SwitchWeapons(newIndex);
            yield return new WaitForSeconds(switchDelay);
            isSwitching = false;
        }

        
        // ���� �Ѵ� ����� �ȵ�
        private void SwitchWeapons(int newIndex)
        {
            for (int i = 0; i < weapon.Length; i++)
            {
                weapon[i].SetActive(false);
            }
            weapon[newIndex].SetActive(true);
        }
        */

        /*
        public void WeaponSOLoader()
        {
            WeaponSO[] weaponSO_arr = Resources.LoadAll<WeaponSO>("WeaponSos");

            foreach(var weaponSO in weaponSO_arr)
            {
                weaponSOs.Add(weaponSO.weaponName, weaponSO);
            }

        }
        */




    }
}
