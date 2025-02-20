using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UIElements;
using System;

namespace HA
{
    public class PlayerData : CharacterBase
    {
        


        [Header("Skill Information")]
        public SkillSlot[] skillSlots;
        public List<SkillSlot> currentActiveSkills;
        public GameObject skillTreeUI;
        public GameObject currentActivateSkillsUI;
        public GameObject[] currentActivateSkillsUI_Slots;

        [Header("Weapon Information")]
        public WeaponSO[] weaponSOs;
        public WeaponSO[] weaponArchive_OfThisCharacter;
        public Weapon[] weaponQuickSlot_OfThisCharacter = new Weapon[2];
        public Weapon currentWeapon;
        public Dictionary<string, WeaponCache_TEMP> weaponCache = new Dictionary<string, WeaponCache_TEMP>();
        public GameObject weaponPrefab_TEMPContainer;


        public int level = 1;
        public float exp { get; set; }
        public float expNeededOnPreviousLevel;
        public float expToNextLevel = 350f; // �������� �ʿ��� ����ġ
        public float expGrowthRate = 1.5f; // ������ �� ���� �������� �ʿ��� ����ġ�� ���� ����
        public int skillPoint;
        public float attack_multiplier { get; set; }
        public float defense_multiplier {  get; set; }
        public float atkRange { get; set; }



        public AudioSource playerAudioPlayer;
        public AudioClip deathSoundClip;
        public AudioClip hitSoundClip;
        public AudioClip itemPickupClip;


        private Animator playerAnimator;
        private PlayerController playerController;

        public CharacterMode characterMode;
        public Color characterColor;

        private float savedHP = -1; // -1�� �ʱ�ȭ�Ͽ� ����� ���� ������ Ȯ��
        private float savedMP = -1;

        private void Awake()
        {
            playerAnimator = GetComponentInChildren<Animator>();
            playerAudioPlayer = GetComponent<AudioSource>();
            playerController = GetComponent<PlayerController>();

            skillSlots = SkillManager.Instance.SkillSlotUIInitialize(characterMode);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            playerController.enabled = true;
            playerAnimator.applyRootMotion = false;

            // savedHP �Ǵ� savedMP�� ����(-1)���, �ʱ�ȭ���� ���� ������ �����Ͽ� �⺻���� ���
            if (savedHP > 0)
            {
                currentHP = savedHP;
            }
            else
            {
                currentHP = initialHealth; // �⺻ HP�� �ʱ�ȭ
            }

            if (savedMP > 0)
            {
                currentMP = savedMP;
            }
            else
            {
                currentMP = initialMp; // �⺻ MP�� �ʱ�ȭ
            }

            OnChangedHP?.Invoke(currentHP, maxHP);
            OnChangedMP?.Invoke(currentMP, maxMP);
        }

        public void GetExp(float expPoints)
        {
            exp += expPoints;

            // ����ġ�� ���� �������� �ʿ��� ����ġ�� �ʰ��ߴ��� Ȯ��
            if (exp >= expToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++; // ���� ����
            expNeededOnPreviousLevel = Mathf.Clamp(exp, expNeededOnPreviousLevel, expToNextLevel);
            // exp = exp - expToNextLevel; // ���� ����ġ ó��
            expToNextLevel *= expGrowthRate; // ���� �������� �ʿ��� ����ġ ����

            // ������ �� HP, ���ݷ� �� ���� ����

            foreach (var character in GameManager.Instance.characters)
            {
                var playerData = character.GetComponent<PlayerData>();
                playerData.maxHP += 10 * (level * expGrowthRate);
                playerData.currentHP = playerData.maxHP;
                playerData.attack_multiplier += 5 * (level * expGrowthRate);
            }


            // ���� UI ������Ʈ
            IngameHUD.Instance.UpdateLevelUI();
            InventoryManager.Instance.StatLevelUpdate();

        }

        public void SaveCharacterState()
        {
            savedHP = currentHP;
            savedMP = currentMP;
        }

        public void RestoreCharacterState()
        {
            // ���¸� ����
            currentHP = savedHP > 0 ? savedHP : initialHealth;
            currentMP = savedMP > 0 ? savedMP : maxMP;
            OnChangedHP?.Invoke(currentHP, maxHP);
            OnChangedMP?.Invoke(currentMP, maxMP);
        }

        [PunRPC]
        public override void IncreaseHP(float amount)
        {
            base.IncreaseHP(amount);
        }

        [PunRPC]
        public override void Damage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (!Dead)
            {
                playerAudioPlayer.PlayOneShot(hitSoundClip);
            }

            base.Damage(damage, hitPoint, hitNormal);
        }

        public override void Die()
        {
            base.Die();

            playerAudioPlayer.PlayOneShot(deathSoundClip);
            playerAnimator.SetTrigger("Die");

            playerAnimator.applyRootMotion = true;
            playerController.enabled = false;

            if (PhotonNetwork.IsConnected)
            {
                // ������(�¶��� ��Ƽ�÷��̾� ����)
                Invoke("Respawn", 5f);
            }

        }

        public void Respawn()
        {
            if (photonView.IsMine)
            {
                savedHP = -1;
                savedMP = -1;

                //Vector3 randomSpawnPosition = Random.insideUnitSphere * 5f;
                //randomSpawnPosition.y = 0f;

                //transform.position = randomSpawnPosition;
            }

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }



        

        


    }

    
}