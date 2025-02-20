using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HA
{
    public class InventoryDataModel : Singleton<InventoryDataModel>
    {
        private string savePath => Path.Combine(Application.persistentDataPath, "inventory.json");
        private bool isInitialized = false;
        public InventoryDataDTO inventoryData;

        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            inventoryData = LoadInventoryData();
        }

        /// <summary>
        /// ���� �κ��丮 �����͸� JSON���� ����
        /// </summary>
        public void SaveInventoryData()
        {
            InventoryDataDTO inventoryDataDTO = new InventoryDataDTO();

            // �κ��丮 ������ ����
            foreach (var slot in InventoryManager.Instance.itemSlot)
            {
                if (slot.quantity > 0)
                {
                    inventoryDataDTO.items.Add(new InventoryItemDTO
                    {
                        itemSO = slot.itemSO,
                        quantity = slot.quantity
                    });
                }
            }

            // (�������� �ƴ�)��� ������ ����
            foreach (var slot in InventoryManager.Instance.equipmentSlot)
            {
                if (slot.quantity > 0)
                {
                    inventoryDataDTO.equipment_NotInUse.Add(new EquipmentItem_NotInUseDTO
                    {
                        equipmentSO = slot.equipmentSO,
                        quantity = slot.quantity,
                    });
                }
            }

            // (��������)��� ������ ����
            for(int i = 0; i < InventoryManager.Instance.equippedSlot.Length; i++)
            {
                if(InventoryManager.Instance.equippedSlot[i].slotInUse)
                {
                    inventoryDataDTO.equipment_InUse.Add(new EquipmentItem_InUseDTO
                    {
                        equipmentSO = InventoryManager.Instance.equippedSlot[i].equipmentSO,
                        equippedSlotNumber = i
                    });
                }
            }

            // JSON ��ȯ �� ����
            string json = JsonUtility.ToJson(inventoryDataDTO, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"[InventoryDataModel] �κ��丮 ���� �Ϸ�: {savePath}");
        }

        /// <summary>
        /// ����� �κ��丮 �����͸� �ҷ��� ����
        /// </summary>
        public InventoryDataDTO LoadInventoryData()
        {
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("[InventoryDataModel] ����� �κ��丮 �����Ͱ� �����ϴ�.");
                return new InventoryDataDTO(); // �� ������ ��ȯ
            }

            string json = File.ReadAllText(savePath);
            InventoryDataDTO inventoryData = JsonUtility.FromJson<InventoryDataDTO>(json);
         

            Debug.Log("[InventoryDataModel] �κ��丮 ������ �ҷ����� �Ϸ�");
            return inventoryData;
        }
    }
}
