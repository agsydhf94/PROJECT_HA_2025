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
        public string itemName;          // 장비 이름
        public string itemDescription;   // 장비 설명
        public ItemType itemType;        // 장비 타입 (Head, Body, Weapon 등)
        public Sprite itemSprite;
        public int price;

        public float attack;
        public float defense;

        [Header("Single Equipment_Prefab And Offsets")]
        public GameObject single_EquipmentPrefab;
        public string single_parentTAG;
        public Vector3 positionOffset;  // 손에 장착될 위치 오프셋
        public Vector3 rotationOffset;  // 손에 장착될 회전 오프셋
        public Vector3 scaleOffset = Vector3.one; // 스케일 조정값

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
            // 플레이어 Stat 업데이트
            PlayerStats playerStats = PlayerStats.Instance;
            playerStats.attack += attack;
            playerStats.defense += defense;

            playerStats.UpdateEquipmentStats();
        }

        public void UnEquipPart_PlayerStatReset()
        {
            // 플레이어 Stat 업데이트
            PlayerStats playerStats = PlayerStats.Instance;
            playerStats.attack -= attack;
            playerStats.defense -= defense;

            playerStats.UpdateEquipmentStats();
        }


        

    }

    
}
