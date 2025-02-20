using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Interaction_Item : MonoBehaviour, IInteractable
    {

         public InventoryItem inventoryItem_copy;

        [SerializeField]
        private string itemName;


        public string Key => "ItemBox." + gameObject.GetHashCode();

        public string Message => $"{itemName}";

        public virtual void Interact()
        {
            // to do : Add item to inventory
            // InventoryItem myItem = GetComponent<InventoryItem>();
            
            // myItem.CopyInventoryItem(inventoryItem_copy);


            // 인벤토리에 아이템 추가
            // GameMaster.instance.inventory.AddItem(myItem);

            // 화면상에서 아이템 제거 및 UI에서 제거
            Destroy(gameObject);
            // InteractionUI.Instance.RemoveInteractionData(this);
        }
    }
}
