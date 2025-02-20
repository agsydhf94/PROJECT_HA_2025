using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace HA
{
    public class EquipmentSO_DataModel : MonoBehaviour
    {
        //public string jsonFileName = "equipment_data"; // Resources ���� �� JSON ���� �̸�
        //public List<EquipmentSO_DataLoader> equipmentDatabase = new List<EquipmentSO_DataLoader>();

        public string jsonFolderName = "JsonData"; // JSON ������ ����� ����
        public string jsonFileName = "EquipmentSOData.json"; // ��ȯ�� JSON ���ϸ�
        public EquipmentSODatabase equipmentDatabase; // EquipmentSO�� ������ �����ͺ��̽�

        public void LoadEquipmentData()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFolderName, jsonFileName);

            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"JSON ������ ã�� �� �����ϴ�: {jsonPath}");
                return;
            }

            string jsonData = File.ReadAllText(jsonPath);
            List<EquipmentSO_DTO> equipmentList = JsonConvert.DeserializeObject<List<EquipmentSO_DTO>>(jsonData);

            foreach (var equipment in equipmentList)
            {
                CreateEquipmentSO(equipment);
            }

            Debug.Log($"��� ������ {equipmentList.Count}���� EquipmentSO�� ��ȯ �Ϸ�!");
        }

        private void CreateEquipmentSO(EquipmentSO_DTO data)
        {
            EquipmentSO newEquipment = ScriptableObject.CreateInstance<EquipmentSO>();

            // �⺻ ���� ����
            newEquipment.itemName = data.itemName;
            newEquipment.itemDescription = data.description;
            newEquipment.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), data.itemType);
            newEquipment.attack = data.attack;
            newEquipment.defense = data.defense;
            newEquipment.single_parentTAG = data.singleParentTag;

            // Vector3 ������ ��ȯ
            newEquipment.positionOffset = new Vector3(data.singlePositionOffsetX, data.singlePositionOffsetY, data.singlePositionOffsetZ);
            newEquipment.rotationOffset = new Vector3(data.singleRotationOffsetX, data.singleRotationOffsetY, data.singleRotationOffsetZ);
            newEquipment.scaleOffset = new Vector3(data.singleScaleOffsetX, data.singleScaleOffsetY, data.singleScaleOffsetZ);

            newEquipment.l_ParentTAG = data.l_ParentTag;
            newEquipment.r_ParentTAG = data.r_ParentTag;
            newEquipment.l_PositionOffset = new Vector3(data.l_PositionOffsetX, data.l_PositionOffsetY, data.l_PositionOffsetZ);
            newEquipment.l_RotationOffset = new Vector3(data.l_RotationOffsetX, data.l_RotationOffsetY, data.l_RotationOffsetZ);
            newEquipment.l_ScaleOffset = new Vector3(data.l_ScaleOffsetX, data.l_ScaleOffsetY, data.l_ScaleOffsetZ);
            newEquipment.r_PositionOffset = new Vector3(data.r_PositionOffsetX, data.r_PositionOffsetY, data.r_PositionOffsetZ);
            newEquipment.r_RotationOffset = new Vector3(data.r_RotationOffsetX, data.r_RotationOffsetY, data.r_RotationOffsetZ);
            newEquipment.r_ScaleOffset = new Vector3(data.r_ScaleOffsetX, data.r_ScaleOffsetY, data.r_ScaleOffsetZ);


            newEquipment.itemSprite = Resources.Load<Sprite>($"Sprites/{data.spriteName}");
            newEquipment.single_EquipmentPrefab = Resources.Load<GameObject>($"Prefabs/{data.singlePrefabName}");
            newEquipment.l_EquipmentPrefab = Resources.Load<GameObject>($"Prefabs/{data.l_PrefabName}");
            newEquipment.r_EquipmentPrefab = Resources.Load<GameObject>($"Prefabs/{data.r_PrefabName}");
            newEquipment.weaponSO = Resources.Load<WeaponSO>($"ScriptableObjects/Weapons/{data.weaponSOPath}");

#if UNITY_EDITOR
            // ������ EquipmentSO�� Assets ������ ����
            string path = $"Assets/Resources/EquipmentSO/{data.itemName}.asset";
            AssetDatabase.CreateAsset(newEquipment, path);
            AssetDatabase.SaveAssets();

            // EquipmentSODatabase�� �߰�
            if (equipmentDatabase != null)
            {
                equipmentDatabase.equipmentSOs.Add(newEquipment);
                EditorUtility.SetDirty(equipmentDatabase);
            }
#endif
            Debug.Log($"EquipmentSO ���� �Ϸ�: {data.itemName}");
        }

    }

}
