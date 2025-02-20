using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class WeaponQuickMenuUI_Manager : MonoBehaviour
    {
        public static WeaponQuickMenuUI_Manager instance;
        public static WeaponQuickMenuUI_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WeaponQuickMenuUI_Manager>();
                }
                return instance;
            }
        }
        [Header("Current Using Weapon EquipmentSO")]
        public EquipmentSO currentWeapon_EquipmentSO;

        
        public GameObject[] weaponQuickMenuUI_Slots;
        public string emptyString = "";
        public Sprite emptySprite;
        public int weaponQuickSlot_index;

        [Header("WeaponQuickMenu UI Animation")]
        public Animator ui_Animator_A;
        public Animator ui_Animator_B;
        public KeyCode toggleKey = KeyCode.V;


        private void Awake()
        {
            weaponQuickSlot_index = 0;
            weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().weaponSelectedImage.gameObject.SetActive(true);

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void WeaponQuickSlot_Reset(int index)
        {
            var weaponSlotUI = weaponQuickMenuUI_Slots[index].GetComponent<WeaponQuickMenuUI_Slot>();
            weaponSlotUI.weaponName.text = emptyString;
            weaponSlotUI.weaponImage.sprite = emptySprite;
            weaponSlotUI.equipmentSO = null;
        }

        void Update()
        {
            // 키보드를 누를 때
            if (Input.GetKey(toggleKey))
            {
                ShowUI();
                WeaponQuickSlot_IndexScroll();
            }
            if(Input.GetKeyUp(toggleKey))
            {
                HideUI();
            }
           
        }

        private void ShowUI()
        {
            ui_Animator_A.SetBool("weaponSlot_A_Show", true);
            ui_Animator_B.SetBool("weaponSlot_B_Show", true);
        }

        private void HideUI()
        {
            ui_Animator_A.SetBool("weaponSlot_A_Show", false);
            ui_Animator_B.SetBool("weaponSlot_B_Show", false);
            weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().SetWeapon_InUse();
        }

        public void WeaponQuickSlot_IndexScroll()
        {
            // index는 현재 퀵 슬롯에 저장된 무기 배열에 쓰일 인덱스
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().weaponSelectedImage.gameObject.SetActive(false);

                weaponQuickSlot_index++;
                if (weaponQuickSlot_index >= GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().weaponQuickSlot_OfThisCharacter.Length)
                {
                    weaponQuickSlot_index = 0;
                }
                weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().weaponSelectedImage.gameObject.SetActive(true);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().weaponSelectedImage.gameObject.SetActive(false);

                weaponQuickSlot_index--;
                if (weaponQuickSlot_index < 0)
                {
                    weaponQuickSlot_index = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().weaponQuickSlot_OfThisCharacter.Length - 1;
                }

                weaponQuickMenuUI_Slots[weaponQuickSlot_index].GetComponent<WeaponQuickMenuUI_Slot>().weaponSelectedImage.gameObject.SetActive(true);

            }
        }
    }
}
