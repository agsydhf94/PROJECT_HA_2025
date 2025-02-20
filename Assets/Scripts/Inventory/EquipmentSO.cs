using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    [CreateAssetMenu]
    public class EquipmentSO : ScriptableObject
    {
        public WeaponSO weaponSO;
        public string itemName;          // ��� �̸�
        public string itemDescription;   // ��� ����
        public ItemType itemType;        // ��� Ÿ�� (Head, Body, Weapon ��)
        public Sprite itemSprite;
        public int price;

        public float attack;
        public float defense;

        [Header("Single Equipment_Prefab And Offsets")]
        public GameObject single_EquipmentPrefab;
        public string single_parentTAG;
        public Vector3 positionOffset;  // �տ� ������ ��ġ ������
        public Vector3 rotationOffset;  // �տ� ������ ȸ�� ������
        public Vector3 scaleOffset = Vector3.one; // ������ ������

        [Header("Dual Equipment_Prefabs And Offsets")]
        public GameObject l_EquipmentPrefab;
        public GameObject r_EquipmentPrefab;
        public string l_ParentTAG;
        public string r_ParentTAG;
        public Vector3 l_PositionOffset;
        public Vector3 l_RotationOffset;
        public Vector3 l_ScaleOffset;
        public Vector3 r_PositionOffset;
        public Vector3 r_RotationOffset;
        public Vector3 r_ScaleOffset;

        public void PreviewEquipment()
        {
            PlayerStats.Instance.EquipmentPreview(attack, defense, itemSprite);
        }

        public void EquipPart_PlayerStatUpdate()
        {
            // �÷��̾� Stat ������Ʈ
            PlayerStats playerStats = PlayerStats.Instance;
            playerStats.attack += attack;
            playerStats.defense += defense;

            playerStats.UpdateEquipmentStats();
        }

        public void UnEquipPart_PlayerStatReset()
        {
            // �÷��̾� Stat ������Ʈ
            PlayerStats playerStats = PlayerStats.Instance;
            playerStats.attack -= attack;
            playerStats.defense -= defense;

            playerStats.UpdateEquipmentStats();
        }


        

    }

    
}
