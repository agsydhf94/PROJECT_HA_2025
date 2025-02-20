using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class PlayerProfile : MonoBehaviour
    {
        public string playerName;
        public float exp;
        public int money;
        public int level;

        public float maxHp { get; set; }
        public float nowHp { get; set; }
        public float maxMp { get; set; }
        public float nowMp { get; set; }
        public float atkDmg { get; set; }

        // �κ��丮 ������ ����
        public List<ItemData> inventoryItems = new List<ItemData>();
        public List<ItemData> equipmentItems = new List<ItemData>();

        // �⺻ ������
        public PlayerProfile(string name, float maxHp, float nowHp, float maxMp, float nowMp, float atkDmg
                            , float exp, int level, int money )
        {
            playerName = name;
            this.maxHp = maxHp;
            this.nowHp = nowHp;
            this.maxMp = maxMp;
            this.nowMp = nowMp;
            this.atkDmg = atkDmg;
            this.exp = exp;
            this.level = level;
            this.money = money;
        }

        // �κ��丮�� ������ �߰�
        public void AddItemToInventory(ItemData item)
        {
            inventoryItems.Add(item);
        }

        // ��� ������ �߰�
        public void EquipItem(ItemData item)
        {
            equipmentItems.Add(item);
        }

        // �κ��丮���� ������ ����
        public void RemoveItemFromInventory(ItemData item)
        {
            inventoryItems.Remove(item);
        }

        // ��񿡼� ������ ����
        public void UnequipItem(ItemData item)
        {
            equipmentItems.Remove(item);
        }
    }

    // ������ ������ ����
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public int quantity;
        public string itemDescription;
        public ItemType itemType;

        // �߰��� ������ �ʿ��� ������...
    }
}
