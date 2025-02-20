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
            // ������ ������ �ε�
            if (inventoryData.items.Count > 0)
            {
                foreach (var item in inventoryData.items)
                {
                    AddItem(item.itemSO, item.quantity);
                }
            }

            // (�������� �ƴ�)��� ������ �ε�
            if (inventoryData.equipment_NotInUse.Count > 0)
            {
                foreach (var equipment in inventoryData.equipment_NotInUse)
                {
                    AddEquipment(equipment.equipmentSO, equipment.quantity);
                }
            }

            // (��������)��� ������ �ε�
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
            // ��: Resources/ItemPrefabs �������� ��� ������ �ε�
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
            // ��ġ������ ��� ���� TransformOffsetData Ŭ������ ��üȭ�ϰ�
            // �ҷ��� weaponSO �κ��� ��� Transform �� ������ ������ �ִ´�
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
            
           
            // ī�װ��� � ��� ���� ������ ����� �� ����
            EquipmentStrategy_Manager.Instance.AutoSelectStrategy(equipmentSO.itemType);

            // ���õ� ��� ���� ������ ����
            EquipmentStrategy_Manager.Instance.Equip(equipmentSO, transformOffsetData_OfThisEquipment);
        }

        /*
        public void EquipItem(EquipmentSO equipmentSO, Transform[] attachPoints)
        {
            // ������ �������� ���ҽ����� ��������
            GameObject itemPrefab = equipmentSO.

            if (itemPrefab != null)
            {
                // EquipStrategy�� ����Ͽ� ��� �����ϴ� ���� ó��
                if (equipmentManager != null)
                {
                    // ���� ���� ����, itemPrefab ����
                    equipmentManager.Equip(itemType, itemName, attachPoints);
                }
                else
                {
                    Debug.LogError("EquipmentManager�� �������� �ʾҽ��ϴ�.");
                }
            }
            else
            {
                Debug.LogError($"������ '{itemName}'��(��) ã�� �� �����ϴ�.");
            }
        }
        */


        // �������̳� ��� ������� ��, ������ ��������ִ� �Լ� ���� �ʿ�
        // �Ͻ������� �Ŀ����� �����ִ� �����ۿ� ���� ���� ��ȹ �ʿ�(������ ���� �ִ� ������ �˰� ����)
        // �������̳� ����� �ʵ带 ������ �������� ���� �ƴ϶� Data ���·� ������ �ͼ� �޼��� ���ο��� ���ľ� ��


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

