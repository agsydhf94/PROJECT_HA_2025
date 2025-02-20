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

        // 인벤토리 데이터 구조
        public List<ItemData> inventoryItems = new List<ItemData>();
        public List<ItemData> equipmentItems = new List<ItemData>();

        // 기본 생성자
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

        // 인벤토리에 아이템 추가
        public void AddItemToInventory(ItemData item)
        {
            inventoryItems.Add(item);
        }

        // 장비에 아이템 추가
        public void EquipItem(ItemData item)
        {
            equipmentItems.Add(item);
        }

        // 인벤토리에서 아이템 삭제
        public void RemoveItemFromInventory(ItemData item)
        {
            inventoryItems.Remove(item);
        }

        // 장비에서 아이템 삭제
        public void UnequipItem(ItemData item)
        {
            equipmentItems.Remove(item);
        }
    }

    // 아이템 데이터 구조
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public int quantity;
        public string itemDescription;
        public ItemType itemType;

        // 추가로 저장할 필요한 정보들...
    }
}
