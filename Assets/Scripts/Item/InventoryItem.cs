using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class InventoryItem : ItemDataBase
    {
        [SerializeField]
        private float weight;

        public ItemCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Name
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public string Description
        {
            get { return itemDescription; }
            set { itemDescription = value; }
        }

        
        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public void CopyInventoryItem(InventoryItem myItem)
        {
            Debug.Log(myItem.Category);
            Category = myItem.Category;
            Description = myItem.itemDescription;
            Name = myItem.itemName;
            Weight = myItem.Weight;
        }
    }
}

