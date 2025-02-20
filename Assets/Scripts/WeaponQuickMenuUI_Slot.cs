using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HA;
using TMPro;
using UnityEngine.UI;
using System;

namespace HA
{
    public class WeaponQuickMenuUI_Slot : MonoBehaviour
    {
        public int slotNumber;

        [Header("Equipment SO")]
        public EquipmentSO equipmentSO;

        [Header("Slot UI Elements")]
        public TMP_Text weaponName;
        public Image weaponImage;
        public Transform weaponQuickMenuPanel_Transform;
        public Image weaponSelectedImage;
        public bool isWeaponUsing;

        // 슬롯이 프리팹화 되어있지 않고 상시 존재하는 유형이므로
        // 무기가 인벤토리로부터 선택될 때 로딩되어야 함

        
        public void SetWeapon_InUse()
        {
            ObjectPool.Instance.CreatePool<ParticleSystem>(equipmentSO.weaponSO.bulletImpactParticle.name,
                                                            equipmentSO.weaponSO.bulletImpactParticle, 15);

            if(equipmentSO != null)
            {
                
                var currentPlayerData = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>();

                if(!currentPlayerData.weaponCache.ContainsKey(equipmentSO.name))
                {

                    if(currentPlayerData.weaponPrefab_TEMPContainer)
                    {
                        currentPlayerData.weaponCache[WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO.name] = currentPlayerData.currentWeapon.WeaponCache;

                        if (!equipmentSO.name.Equals(WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO.name))
                        {
                            InventoryManager.Instance.EquipGear_OnToBody(equipmentSO);
                        }

                        var __newWeapon = currentPlayerData.weaponPrefab_TEMPContainer.GetComponent<Weapon>();
                        __newWeapon.weaponSO = equipmentSO.weaponSO;
                        __newWeapon.WeaponData_Initialize();
                        WeaponCache_TEMP weaponCache = __newWeapon.WeaponCache;
                        currentPlayerData.weaponCache[equipmentSO.name] = weaponCache;
                    }
                    else
                    {
                        InventoryManager.Instance.EquipGear_OnToBody(equipmentSO);
                        var _newWeapon = currentPlayerData.weaponPrefab_TEMPContainer.GetComponent<Weapon>();
                        
                        if(_newWeapon.weaponSO == null)
                        {
                            _newWeapon.weaponSO = equipmentSO.weaponSO;
                            _newWeapon.WeaponData_Initialize();
                        }                        
                        currentPlayerData.weaponCache[equipmentSO.name] = _newWeapon.WeaponCache;
                        
                    }
                    
                    

                    var newWeapon = currentPlayerData.weaponPrefab_TEMPContainer.GetComponent<Weapon>();
                    newWeapon.weaponSO = equipmentSO.weaponSO;
                    currentPlayerData.currentWeapon = newWeapon;
                    currentPlayerData.currentWeapon.WeaponCache = currentPlayerData.weaponCache[equipmentSO.name];
                    currentPlayerData.weaponCache[equipmentSO.name] = currentPlayerData.currentWeapon.WeaponCache;

                    WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO = equipmentSO;

                    var weaponPrefab = currentPlayerData.weaponPrefab_TEMPContainer;
                    if(PlayerController.Instance.isArmed)
                    {
                        weaponPrefab.SetActive(true);
                    }
                    else
                    {
                        weaponPrefab.SetActive(false);
                    }
                    
                }
                else
                {
                    if(!equipmentSO.name.Equals(WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO.name))
                    {
                        currentPlayerData.weaponCache[WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO.name] = currentPlayerData.currentWeapon.WeaponCache;
                        InventoryManager.Instance.EquipGear_OnToBody(equipmentSO);

                        var newWeapon = currentPlayerData.weaponPrefab_TEMPContainer.GetComponent<Weapon>();
                        currentPlayerData.currentWeapon = newWeapon;
                        currentPlayerData.currentWeapon.weaponSO = equipmentSO.weaponSO;
                        currentPlayerData.currentWeapon.WeaponCache = currentPlayerData.weaponCache[equipmentSO.name];
                        WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO = equipmentSO;
                    }                    
                }

            }
            
        }       
    }
}
