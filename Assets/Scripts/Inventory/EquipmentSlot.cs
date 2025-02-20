using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
    {
        // Item data
        public string itemName; // SO
        public int quantity;
        public Sprite itemSprite; // SO 
        public bool isFull;
        public string itemDescription;
        public Sprite emptySprite;
        public ItemType itemType;

        public EquipmentSO equipmentSO;


        [SerializeField]
        private Image itemImage;

        // 현재 플레이어가 장착하고 있는 파츠
        public EquippedSlot headSlot;
        public EquippedSlot bodySlot;
        public EquippedSlot armSlot;
        public EquippedSlot legsSlot;
        public EquippedSlot[] rifleSlot;
        public EquippedSlot handgunSlot;



        public GameObject selectedShader;
        public bool thisItemSelected;

        private InventoryManager inventoryManager;
        private EquipmentSODatabase equipmentSODatabase;
        private PlayerStats playerStats;


        private void Start()
        {
            inventoryManager = InventoryManager.Instance;
            equipmentSODatabase = EquipmentSODatabase.Instance;
            playerStats = PlayerStats.Instance;
        }

        public int AddItem_Into_EquipmentSlot(EquipmentSO _equipmentSO, int quantity)
        {
            // Check to see if the slot if already full
            if (isFull)
            {
                return quantity;
            }

            equipmentSO = _equipmentSO;

            itemName = equipmentSO.itemName;
            itemDescription = equipmentSO.itemDescription;

            itemSprite = equipmentSO.itemSprite;
            itemImage.sprite = itemSprite;

            itemType = equipmentSO.itemType;

            // update Quantity
            this.quantity = 1;
            isFull = true;
            return 0;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
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
            if(isFull)
            {
                if (thisItemSelected)
                {
                    EquipGear();

                }
                else
                {
                    inventoryManager.DeselectAllSlots();
                    selectedShader.SetActive(true);
                    thisItemSelected = true;

                    for (int i = 0; i < equipmentSODatabase.equipmentSOs.Count; i++)
                    {
                        Debug.Log($"이것의 이름 : {this.itemName} /// so의 이름 : {equipmentSODatabase.equipmentSOs[i].itemName}");
                        if (equipmentSODatabase.equipmentSOs[i].itemName == this.itemName)
                        {
                            equipmentSODatabase.equipmentSOs[i].PreviewEquipment();
                        }
                    }
                }
            }
            else
            {
                playerStats.ActivePreview();
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;
            }

            
        }

        
        private void EquipGear()
        {
            if(itemType == ItemType.head)
            {
                headSlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            if (itemType == ItemType.body)
            {
                bodySlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            if (itemType == ItemType.arm)
            {
                armSlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            if (itemType == ItemType.legs)
            {
                legsSlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            /*
            if (itemType == ItemType.Rifle)
            {
                rifleSlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            if (itemType == ItemType.Handgun)
            {
                handgunSlot.EquipGear_To_EquippedSlot(equipmentSO);
            }
            */

            if(itemType == ItemType.Weapon)
            {
                foreach(var rifleSlot in rifleSlot)
                {
                    if(rifleSlot.equipmentSO == null)
                    {
                        rifleSlot.EquipGear_To_EquippedSlot(equipmentSO);
                        break;
                    }
                }
            }

            EmptySlot();
        }
        

        private void EmptySlot()
        {
            itemName = "";
            quantity = 0;
            itemImage.sprite = emptySprite;
            isFull = false;
            itemDescription = "";
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

            SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprite;
            sr.sortingOrder = 5;
            sr.sortingLayerName = "Ground";

            itemToDrop.AddComponent<BoxCollider>();

            itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position
                + new Vector3(0.5f, 0, 0);
            itemToDrop.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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

                
            }
            else
            {
                Debug.LogError($"Prefab for item '{itemName}' not found.");
            }


            // Item Subtraction

            this.quantity -= 1;
            EmptySlot();
        }
    }
}

