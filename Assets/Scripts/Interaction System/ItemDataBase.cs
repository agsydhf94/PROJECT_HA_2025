using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class ItemDataBase
    {
        public string itemID;
        public string itemName;
        public string itemDescription;
        public ItemCategory category;

        
    }

    public class WeaponItemData : ItemDataBase
    {
        public int remainAmmo;
    }

    public class ArmourItemData : ItemDataBase
    {
        public int protectPoint;
    }

    public class ClothItemData : ItemDataBase
    {
        public int remainDurability;
    }

    public class HealthItemData : ItemDataBase
    {
        public float recoverHP;
    }

    public class PotionItemData : ItemDataBase
    {
        public int recoverMP;
    }


}

