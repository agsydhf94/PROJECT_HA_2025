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
        /// 현재 인벤토리 데이터를 JSON으로 저장
        /// </summary>
        public void SaveInventoryData()
        {
            InventoryDataDTO inventoryDataDTO = new InventoryDataDTO();

            // 인벤토리 아이템 저장
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

            // (장착중이 아닌)장비 아이템 저장
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

            // (장착중인)장비 아이템 저장
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

            // JSON 변환 및 저장
            string json = JsonUtility.ToJson(inventoryDataDTO, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"[InventoryDataModel] 인벤토리 저장 완료: {savePath}");
        }

        /// <summary>
        /// 저장된 인벤토리 데이터를 불러와 적용
        /// </summary>
        public InventoryDataDTO LoadInventoryData()
        {
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("[InventoryDataModel] 저장된 인벤토리 데이터가 없습니다.");
                return new InventoryDataDTO(); // 빈 데이터 반환
            }

            string json = File.ReadAllText(savePath);
            InventoryDataDTO inventoryData = JsonUtility.FromJson<InventoryDataDTO>(json);
         

            Debug.Log("[InventoryDataModel] 인벤토리 데이터 불러오기 완료");
            return inventoryData;
        }
    }
}
