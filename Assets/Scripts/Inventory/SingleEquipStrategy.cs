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
            // InventoryManager���� �̹� �������� �ҷ��Ա� ������, ���⼭�� attachPoints�� ������ �ϸ� �˴ϴ�
            // GameObject itemPrefab = InventoryManager.Instance.GetItemPrefab(equipmentSO.itemName);
            GameObject single_EquipmentPrefab = equipmentSO.single_EquipmentPrefab;
            
            // ���� ��� �����ϴ� �޼����̹Ƿ�, �ϳ��� ������ġ ������ ��缱��
            Transform parentTransform = GameObject.FindGameObjectWithTag(transformOffsetData.single_parentTAG).transform;


            if (single_EquipmentPrefab != null)
            {
                if (parentTransform.childCount > 0)
                {
                    Destroy(parentTransform.GetChild(0).gameObject);
                }

                // ���ο� ��� ����
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
                Debug.LogError($"������ '{equipmentSO.itemName}'�� �ش��ϴ� �������� ã�� �� �����ϴ�.");
            }

        }
    }
}
