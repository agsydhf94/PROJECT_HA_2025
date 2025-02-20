using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class EquippedSlot : MonoBehaviour, IPointerClickHandler
    {
        // Slot Shape
        [SerializeField]
        private Image slotImage;

        [SerializeField]
        private TMP_Text slotName;


        // Slot Data
        [SerializeField]
        private ItemType itemType = new ItemType();

        [SerializeField]
        public GameObject selectedShader;

        [SerializeField]
        public bool thisItemSelected;

        [SerializeField]
        private Sprite emptySprite;


        public Sprite itemSprite;  // 인벤토리 메뉴 창에 뜰 아이템 이미지
        public string itemName;
        public string itemDescription;
        public EquipmentSO equipmentSO;

        public bool slotInUse;

        private InventoryManager inventoryManager;
        private EquipmentStrategy_Manager equipmentManager;
        private EquipmentSODatabase equipmentSODatabase;
        private PlayerStats playerStats;

        private void Start()
        {
            inventoryManager = InventoryManager.Instance;
            equipmentManager = EquipmentStrategy_Manager.Instance;
            equipmentSODatabase = EquipmentSODatabase.Instance;
            playerStats = PlayerStats.Instance; 
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick();
            }
            
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }

        public void OnLeftClick()
        {
            if(thisItemSelected && slotInUse)
            {
                if(!equipmentSO.itemName.Equals(WeaponQuickMenuUI_Manager.Instance.currentWeapon_EquipmentSO.itemName))
                {
                    UnEquipGear();
                }
                
            }
            else
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;

                for (int i = 0; i < equipmentSODatabase.equipmentSOs.Count; i++)
                {                    
                    if (equipmentSODatabase.equipmentSOs[i].itemName == this.itemName)
                    {
                        equipmentSODatabase.equipmentSOs[i].PreviewEquipment();
                    }
                }
            }
        }

        public void OnRightClick()
        {
            UnEquipGear();
        }


        public void EquipGear_To_EquippedSlot(EquipmentSO _equipmentSO)
        {
            if(slotInUse)
            {
                UnEquipGear();
            }

            equipmentSO = _equipmentSO;
            Debug.Log("equipmentSO 불러오기 완료");

            // Image update
            itemSprite = _equipmentSO.itemSprite;
            slotImage.sprite = itemSprite;
            slotName.enabled = false;

            // Name, Description update
            itemName = _equipmentSO.itemName;
            itemDescription = _equipmentSO.itemDescription;
            slotInUse = true;

            // Stat update
            for(int i = 0; i < equipmentSODatabase.equipmentSOs.Count; i++)
            {
                if (equipmentSODatabase.equipmentSOs[i].itemName == this.itemName)
                {
                    equipmentSODatabase.equipmentSOs[i].EquipPart_PlayerStatUpdate();
                }
            }


            if(equipmentSO.itemType == ItemType.Weapon)
            {
                // 일단 현재 사용중인 무기 슬롯으로 저장
                foreach(var weaponQuickSlot in WeaponQuickMenuUI_Manager.Instance.weaponQuickMenuUI_Slots)
                {
                    var weaponQuickSlot_Component = weaponQuickSlot.GetComponent<WeaponQuickMenuUI_Slot>();
                    if (weaponQuickSlot_Component.equipmentSO == null)
                    {
                        weaponQuickSlot_Component.equipmentSO = equipmentSO;
                        weaponQuickSlot_Component.weaponName.text = equipmentSO.weaponSO.name;
                        weaponQuickSlot_Component.weaponImage.sprite = equipmentSO.weaponSO.weaponSprite;

                        break;
                    }
                }
            }
            
            else
            {
                Debug.Log("이 부분 실행됨");
                InventoryManager.Instance.EquipGear_OnToBody(equipmentSO);
            }

            


        }

        



            //inventoryManager.EquipItem(_equipmentSO, attachPoints);

            // 기존 장착된 아이템 제거
            /*
            if (itemObjects != null)
            {
                foreach (GameObject obj in itemObjects)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            // 아이템 오브젝트 배열 초기화
            itemObjects = new GameObject[attachPoints.Length];

            // 각 부착 포인트에 아이템 부착
            for (int i = 0; i < attachPoints.Length; i++)
            {
                itemObjects[i] = Instantiate(itemPrefab, attachPoints[i].position, attachPoints[i].rotation, attachPoints[i]);
            }
            */

        

        
        public void UnEquipGear()
        {
            inventoryManager.DeselectAllSlots();

            inventoryManager.AddEquipment(equipmentSO, 1);

            slotInUse = false;
            this.itemSprite = emptySprite;
            slotImage.sprite = emptySprite;
            slotName.enabled = true;
            itemName = "";
            itemDescription = "";

            if(equipmentSO.itemType == ItemType.shoulder)
            {
                Transform l_ParentTransform = GameObject.FindGameObjectWithTag(equipmentSO.l_ParentTAG).transform;
                Transform r_ParentTransform = GameObject.FindGameObjectWithTag(equipmentSO.r_ParentTAG).transform;
                Destroy(l_ParentTransform.GetChild(0).gameObject);
                Destroy(r_ParentTransform.GetChild(0).gameObject);
            }

            foreach(var weaponSlot in WeaponQuickMenuUI_Manager.Instance.weaponQuickMenuUI_Slots)
            {
                if(weaponSlot.GetComponent<WeaponQuickMenuUI_Slot>().equipmentSO.itemName.Equals(equipmentSO.itemName))
                {
                    WeaponQuickMenuUI_Manager.Instance.WeaponQuickSlot_Reset(weaponSlot.GetComponent<WeaponQuickMenuUI_Slot>().slotNumber);
                }
            }
            

            // Stat update
            for (int i = 0; i < equipmentSODatabase.equipmentSOs.Count; i++)
            {
                if (equipmentSODatabase.equipmentSOs[i].itemName == this.itemName)
                {
                    equipmentSODatabase.equipmentSOs[i].UnEquipPart_PlayerStatReset();
                }
            }

            playerStats.ActivePreview();
            equipmentSO = null;
        }
        
    }
}
