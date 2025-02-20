using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HA
{
    public class Item : MonoBehaviour, IInteractable
    {
        [Header("Item Scriptable Object")]
        public ItemSO itemSO;
        public int quantity;


        public string Key => gameObject.name;
        public string Message => $"{itemSO.itemName}";





        public void Interact()
        {

            if (InventoryManager.Instance == null)
                Debug.Log("InventoryManager.Instance == null");

            if (itemSO == null)
                Debug.Log("itemSO == null");

            if (quantity == null)
                Debug.Log("quantity == null");

            int leftOverItems = InventoryManager.Instance.AddItem(itemSO, quantity);
            if (leftOverItems <= 0)
            {
                InteractionUI.Instance.RemoveInteractionData(this);
                Destroy(gameObject);
            }
            else
            {
                quantity = leftOverItems;
            }
        }


    }

}
