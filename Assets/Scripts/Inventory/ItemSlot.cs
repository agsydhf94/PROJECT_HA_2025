using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        // Item data
        public string itemName;
        public int quantity;
        public Sprite itemSprite;
        public bool isFull;
        public string itemDescription;
        public Sprite emptySprite;
        public ItemType itemType;
        public ItemSO itemSO;

        [SerializeField]
        private int maxNumberOfItems;

        // Item Slot
        [SerializeField]
        private TMP_Text quantityText;

        [SerializeField]
        private Image itemImage;


        // Item Description
        public Image itemDescriptionImage;
        public TMP_Text ItemDescriptionNameText;
        public TMP_Text ItemDescriptionText;


        public GameObject selectedShader;
        public bool thisItemSelected;

        private InventoryManager inventoryManager;

        private void Start()
        {
            // inventoryManager = GameObject.Find("HA.InventoryUI").GetComponent<InventoryManager>();
            // inventoryManager = GameObject.FindGameObjectWithTag("InventoryUI").GetComponent<InventoryManager>();
            inventoryManager = InventoryManager.Instance;
        }

        public int AddItem_Into_ItemSlot(ItemSO _itemSO, int quantity)
        {
            // Check to see if the slot if already full
            if(isFull)
            {
                return quantity;
            }

            itemSO = _itemSO;

            
            itemName = itemSO.itemName;
            itemDescription = itemSO.itemDescription;

            itemSprite = itemSO.itemSprite;
            itemImage.sprite = itemSprite;
            
            itemType = itemSO.itemType;

            

            // 수량 갱신
            this.quantity += quantity;
                        

            
            if(this.quantity >= maxNumberOfItems)
            {
                quantityText.text = maxNumberOfItems.ToString();
                quantityText.enabled = true;
                isFull = true;

                // return the leftovers
                int extraItems = this.quantity - maxNumberOfItems;
                this.quantity = maxNumberOfItems;
                return extraItems;
            }

            // update the quantity text
            quantityText.text = this.quantity.ToString();
            quantityText.enabled = true;

            return 0;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick();
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }

        public void OnLeftClick()
        {
            if(thisItemSelected)
            {
                bool usable = inventoryManager.UseItem(itemName);
                if(usable)
                {
                    this.quantity -= 1;
                    quantityText.text = this.quantity.ToString();
                    if (this.quantity <= 0)
                    {
                        EmptySlot();
                    }
                }
                
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
                ItemDescriptionNameText.text = itemName;
                ItemDescriptionText.text = itemDescription;
                itemDescriptionImage.sprite = itemSprite;
                if (itemDescriptionImage.sprite == null)
                {
                    itemDescriptionImage.sprite = emptySprite;
                }
            }      
        }

        private void EmptySlot()
        {
            quantityText.enabled = false;
            itemImage.sprite = emptySprite;

            ItemDescriptionNameText.text = "";
            ItemDescriptionText.text = "";
            itemDescriptionImage.sprite = emptySprite;
        }

        public void OnRightClick()
        {
            /*
            GameObject itemToDrop = new GameObject(itemName);
            Item newItem = itemToDrop.AddComponent<Item>();
            newItem.quantity = 1;
            newItem.itemName = itemName;
            newItem.itemImage = itemSprite;
            newItem.itemDescription = itemDescription;
            newItem.itemPrefab = itemObject;
            */

            /*
            SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprite;
            sr.sortingOrder = 5;
            sr.sortingLayerName = "Ground";
            */


            // InventoryManager에서 해당 아이템의 프리팹을 가져오기
            GameObject itemPrefab = inventoryManager.GetItemPrefab(itemName);

            if (itemPrefab != null)
            {
                GameObject itemToDrop = Instantiate(itemPrefab, GameObject.FindWithTag("Player").transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
                itemToDrop.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                if (!itemToDrop.GetComponent<BoxCollider>())
                {
                    itemToDrop.AddComponent<BoxCollider>();
                }

                if (!itemToDrop.GetComponent<Rigidbody>())
                {
                    itemToDrop.AddComponent<Rigidbody>();
                }

                // 아이템 정보 업데이트 및 감소
                this.quantity -= 1;
                quantityText.text = this.quantity.ToString();
                if (this.quantity <= 0)
                {
                    EmptySlot();
                }
            }
            else
            {
                Debug.LogError($"Prefab for item '{itemName}' not found.");
            }



        }
    }
}
