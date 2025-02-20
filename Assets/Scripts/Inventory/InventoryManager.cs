using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

namespace HA
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public PlayerData playerData;
        public GameObject ItemMenu;
        public ItemSlot[] itemSlot;
        public ItemSO[] itemSOs;



        public GameObject EquipmentMenu;
        public EquipmentSlot[] equipmentSlot;
        public EquippedSlot[] equippedSlot;
        public TMP_Text levelStatText;
        public TMP_Text hpStatText;
        public TMP_Text mpStatText;



        public Dictionary<string, GameObject> itemPrefabs;
        
        public BlurEffectController blurController;

        private bool isInitialized = false;


        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;

            itemPrefabs = new Dictionary<string, GameObject>();
            LoadItemPrefabs();

            playerData = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>();
        }
        /*
        public void InventoryUI_Initialize(InventoryDataDTO inventoryData)
        {
            // 아이템 데이터 로드
            if (inventoryData.items.Count > 0)
            {
                foreach (var item in inventoryData.items)
                {
                    AddItem(item.itemSO, item.quantity);
                }
            }

            // (장착중이 아닌)장비 데이터 로드
            if (inventoryData.equipment_NotInUse.Count > 0)
            {
                foreach (var equipment in inventoryData.equipment_NotInUse)
                {
                    AddEquipment(equipment.equipmentSO, equipment.quantity);
                }
            }

            // (장착중인)장비 데이터 로드
            if (inventoryData.equipment_InUse.Count > 0)
            {
                foreach (var equipment in inventoryData.equipment_InUse)
                {
                    Instance.equippedSlot[equipment.equippedSlotNumber].EquipGear_To_EquippedSlot(equipment.equipmentSO);
                }
            }
        }
        */

        private void LoadItemPrefabs()
        {
            // 예: Resources/ItemPrefabs 폴더에서 모든 프리팹 로드
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Item Resources");

            foreach (GameObject prefab in prefabs)
            {
                itemPrefabs[prefab.name] = prefab;
            }
        }

        public GameObject GetItemPrefab(string itemName)
        {
            if (itemPrefabs.ContainsKey(itemName))
            {
                return itemPrefabs[itemName];
            }
            return null;
        }




        public bool UseItem(string itemName)
        {
            if (!PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < itemSOs.Length; i++)
                {
                    if (itemSOs[i].itemName == itemName)
                    {
                        bool usable = itemSOs[i].UseItem();
                        return usable;
                    }

                }
                return false;
            }
            else
            {
                return false;
            }

        }

        public int AddItem(ItemSO itemSO, int quantity)
        {
            var item = itemSO;

            for (int i = 0; i < itemSlot.Length; i++)
            {
                if (itemSlot[i].isFull == false && itemSlot[i].itemName == item.itemName || itemSlot[i].quantity == 0)
                {
                    int leftOverItems = itemSlot[i].AddItem_Into_ItemSlot(item, quantity);
                    if (leftOverItems > 0)
                    {
                        leftOverItems = AddItem(item, leftOverItems);

                    }
                    return leftOverItems;
                }
            }
            return quantity;            
        }

        public int AddEquipment(EquipmentSO equipmentSO, int quantity)
        {
            var equipment = equipmentSO;

            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                if (equipmentSlot[i].isFull == false && equipmentSlot[i].itemName == equipment.itemName || equipmentSlot[i].quantity == 0)
                {
                    int leftOverItems = equipmentSlot[i].AddItem_Into_EquipmentSlot(equipment, quantity);
                    if (leftOverItems > 0)
                    {
                        leftOverItems = AddEquipment(equipment, leftOverItems);

                    }
                    return leftOverItems;
                }
            }
            return quantity;
        }

        /*
        public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemType itemType)
        {
            if (itemType == ItemType.consumable)
            {
                for (int i = 0; i < itemSlot.Length; i++)
                {
                    if (itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
                    {
                        int leftOverItems = itemSlot[i].AddItem_Into_ItemSlot(itemName, quantity, itemSprite, itemDescription, itemType);
                        if (leftOverItems > 0)
                        {
                            leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription, itemType);

                        }
                        return leftOverItems;
                    }
                }
                return quantity;
            }
            else
            {
                for (int i = 0; i < equipmentSlot.Length; i++)
                {
                    if (equipmentSlot[i].isFull == false && equipmentSlot[i].itemName == itemName || equipmentSlot[i].quantity == 0)
                    {
                        int leftOverItems = equipmentSlot[i].AddItem_Into_EquipmentSlot(itemName, quantity, itemSprite, itemDescription, itemType);
                        if (leftOverItems > 0)
                        {
                            leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription, itemType);

                        }
                        return leftOverItems;
                    }
                }
                return quantity;
            }
        }
        */

        public void DeselectAllSlots()
        {
            for (int i = 0; i < itemSlot.Length; i++)
            {
                itemSlot[i].selectedShader.SetActive(false);
                itemSlot[i].thisItemSelected = false;
            }
            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                equipmentSlot[i].selectedShader.SetActive(false);
                equipmentSlot[i].thisItemSelected = false;
            }
            for (int i = 0; i < equippedSlot.Length; i++)
            {
                equippedSlot[i].selectedShader.SetActive(false);
                equippedSlot[i].thisItemSelected = false;
            }
        }


        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


        public void StatLevelUpdate()
        {
            levelStatText.text = $"{GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().level}";
        }

        public void EquipGear_OnToBody(EquipmentSO equipmentSO)
        {
            // 위치정보를 담기 위한 TransformOffsetData 클래스를 객체화하고
            // 불러온 weaponSO 로부터 장비 Transform 의 오프셋 정보를 넣는다
            TransformOffsetData transformOffsetData_OfThisEquipment = new TransformOffsetData();

            
            transformOffsetData_OfThisEquipment.single_parentTAG = equipmentSO.single_parentTAG;
            transformOffsetData_OfThisEquipment.positionOffset = equipmentSO.positionOffset;
            transformOffsetData_OfThisEquipment.rotationOffset = equipmentSO.rotationOffset;
            transformOffsetData_OfThisEquipment.scaleOffset = equipmentSO.scaleOffset;

            transformOffsetData_OfThisEquipment.l_ParentTAG = equipmentSO.l_ParentTAG;
            transformOffsetData_OfThisEquipment.l_PositionOffset = equipmentSO.l_PositionOffset;
            transformOffsetData_OfThisEquipment.l_RotationOffset = equipmentSO.l_RotationOffset;
            transformOffsetData_OfThisEquipment.l_ScaleOffset = equipmentSO.l_ScaleOffset;
            transformOffsetData_OfThisEquipment.r_ParentTAG = equipmentSO.r_ParentTAG;
            transformOffsetData_OfThisEquipment.r_PositionOffset = equipmentSO.r_PositionOffset;
            transformOffsetData_OfThisEquipment.r_RotationOffset = equipmentSO.r_RotationOffset;
            transformOffsetData_OfThisEquipment.r_ScaleOffset = equipmentSO.r_ScaleOffset;
            
           
            // 카테고리로 어떤 장비 장착 전략을 사용할 지 선택
            EquipmentStrategy_Manager.Instance.AutoSelectStrategy(equipmentSO.itemType);

            // 선택된 장비 장착 전략을 실행
            EquipmentStrategy_Manager.Instance.Equip(equipmentSO, transformOffsetData_OfThisEquipment);
        }

        /*
        public void EquipItem(EquipmentSO equipmentSO, Transform[] attachPoints)
        {
            // 아이템 프리팹을 리소스에서 가져오기
            GameObject itemPrefab = equipmentSO.

            if (itemPrefab != null)
            {
                // EquipStrategy를 사용하여 장비를 장착하는 로직 처리
                if (equipmentManager != null)
                {
                    // 장착 전략 실행, itemPrefab 전달
                    equipmentManager.Equip(itemType, itemName, attachPoints);
                }
                else
                {
                    Debug.LogError("EquipmentManager가 설정되지 않았습니다.");
                }
            }
            else
            {
                Debug.LogError($"아이템 '{itemName}'을(를) 찾을 수 없습니다.");
            }
        }
        */


        // 아이템이나 장비를 사용했을 때, 스탯을 적용시켜주는 함수 구현 필요
        // 일시적으로 파워업을 시켜주는 아이템에 대한 구현 계획 필요(디자인 패턴 있는 것으로 알고 있음)
        // 아이템이나 장비의 필드를 일일히 가져오는 것이 아니라 Data 형태로 가지고 와서 메서드 내부에서 펼쳐야 됨


    }

    

    
}

public enum ItemType
{
    consumable,
    head,
    body,
    shoulder,
    arm,
    legs,
    Weapon,
    none,
}

public enum AttributesToChange
{
    NONE,
    HEALTH,
    MANA,
    RIFLE_BULLET,
    HANDGUN_BULLET,
}

public enum StatToChange
{
    NONE,
    ATTACK,
    DEFENSE
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;

    public ItemType itemType;
    public AttributesToChange statToChange;
    public StatToChange attributesToChange;
}

[System.Serializable]
public struct TransformOffsetData
{
    // Single Equipment
    public string single_parentTAG;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;


    // Dual Equipment
    public string l_ParentTAG;
    public string r_ParentTAG;
    public Vector3 l_PositionOffset;
    public Vector3 l_RotationOffset;
    public Vector3 l_ScaleOffset;
    public Vector3 r_PositionOffset;
    public Vector3 r_RotationOffset;
    public Vector3 r_ScaleOffset;
}

