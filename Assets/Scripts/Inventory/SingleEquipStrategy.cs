using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HA
{
    public class SingleEquipStrategy : MonoBehaviour, IEquipStrategy
    {
        public void Equip(EquipmentSO equipmentSO, TransformOffsetData transformOffsetData)
        {
            // InventoryManager에서 이미 프리팹을 불러왔기 때문에, 여기서는 attachPoints에 장착만 하면 됩니다
            // GameObject itemPrefab = InventoryManager.Instance.GetItemPrefab(equipmentSO.itemName);
            GameObject single_EquipmentPrefab = equipmentSO.single_EquipmentPrefab;
            
            // 단일 장비를 장착하는 메서드이므로, 하나의 장착위치 정보만 취사선택
            Transform parentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.single_parentTAG).transform;


            if (single_EquipmentPrefab != null)
            {
                if (parentTransform.childCount > 0)
                {
                    Destroy(parentTransform.GetChild(0).gameObject);
                }

                // 새로운 장비를 장착
                var newEquipment = Instantiate(single_EquipmentPrefab, parentTransform);
                newEquipment.transform.localPosition = transformOffsetData.positionOffset;
                newEquipment.transform.localRotation = Quaternion.Euler(transformOffsetData.rotationOffset);
                newEquipment.transform.localScale = transformOffsetData.scaleOffset;

                if(equipmentSO.itemType == ItemType.Weapon)
                {
                    var currentPlayerData = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>();
                    if (currentPlayerData.currentWeapon)
                    {
                        currentPlayerData.weaponPrefab_TEMPContainer = null;
                    }                    
                    currentPlayerData.weaponPrefab_TEMPContainer = newEquipment;
                }

            }
            else
            {
                Debug.LogError($"아이템 '{equipmentSO.itemName}'에 해당하는 프리팹을 찾을 수 없습니다.");
            }

        }
    }
}
