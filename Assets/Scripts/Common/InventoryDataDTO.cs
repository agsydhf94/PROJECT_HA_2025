using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class InventoryDataDTO
    {
        public List<InventoryItemDTO> items = new List<InventoryItemDTO>();
        public List<EquipmentItem_NotInUseDTO> equipment_NotInUse = new List<EquipmentItem_NotInUseDTO>();
        public List<EquipmentItem_InUseDTO> equipment_InUse = new List<EquipmentItem_InUseDTO>();
    }

    [System.Serializable]
    public class InventoryItemDTO
    {
        public ItemSO itemSO;
        public int quantity;
    }

    [System.Serializable]
    public class EquipmentItem_NotInUseDTO
    {
        public EquipmentSO equipmentSO;
        public int quantity;
    }

    [System.Serializable]
    public class EquipmentItem_InUseDTO
    {
        public EquipmentSO equipmentSO;
        public int equippedSlotNumber;
    }
}
