using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class EquipmentSO_DTO
    {
        public string itemName;
        public string description;
        public string itemType;
        public string spriteName;
        public int attack;
        public int defense;

        public string weaponSOPath;

        // Single Equipment
        public string singlePrefabName;
        public string singleParentTag;
        public float singlePositionOffsetX;
        public float singlePositionOffsetY;
        public float singlePositionOffsetZ;
        public float singleRotationOffsetX;
        public float singleRotationOffsetY;
        public float singleRotationOffsetZ;
        public float singleScaleOffsetX;
        public float singleScaleOffsetY;
        public float singleScaleOffsetZ;

        // Dual Equipment
        public string l_PrefabName;
        public string r_PrefabName;
        public string l_ParentTag;
        public string r_ParentTag;
        public float l_PositionOffsetX;
        public float l_PositionOffsetY;
        public float l_PositionOffsetZ;
        public float l_RotationOffsetX;
        public float l_RotationOffsetY;
        public float l_RotationOffsetZ;
        public float l_ScaleOffsetX;
        public float l_ScaleOffsetY;
        public float l_ScaleOffsetZ;
        public float r_PositionOffsetX;
        public float r_PositionOffsetY;
        public float r_PositionOffsetZ;
        public float r_RotationOffsetX;
        public float r_RotationOffsetY;
        public float r_RotationOffsetZ;
        public float r_ScaleOffsetX;
        public float r_ScaleOffsetY;
        public float r_ScaleOffsetZ;
    }

}
