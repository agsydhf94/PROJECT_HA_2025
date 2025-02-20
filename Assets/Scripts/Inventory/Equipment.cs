using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Equipment : MonoBehaviour, IInteractable
    {
        [Header("Item Scriptable Object")]
        public EquipmentSO equipmentSO;
        public int quantity;

        public string Key => gameObject.name;
        public string Message => $"{equipmentSO.itemName}";



        public void Interact()
        {
            int leftOverItems = InventoryManager.Instance.AddEquipment(equipmentSO, quantity);
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
