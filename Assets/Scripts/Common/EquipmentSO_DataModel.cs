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
        //public string jsonFileName = "equipment_data"; // Resources 폴더 내 JSON 파일 이름
        //public List<EquipmentSO_DataLoader> equipmentDatabase = new List<EquipmentSO_DataLoader>();

        public string jsonFolderName = "JsonData"; // JSON 파일이 저장된 폴더
        public string jsonFileName = "EquipmentSOData.json"; // 변환할 JSON 파일명
        public EquipmentSODatabase equipmentDatabase; // EquipmentSO를 저장할 데이터베이스

        public void LoadEquipmentData()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFolderName, jsonFileName);

            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"JSON 파일을 찾을 수 없습니다: {jsonPath}");
                return;
            }

            string jsonData = File.ReadAllText(jsonPath);
            List<EquipmentSO_DTO> equipmentList = JsonConvert.DeserializeObject<List<EquipmentSO_DTO>>(jsonData);

            foreach (var equipment in equipmentList)
            {
                CreateEquipmentSO(equipment);
            }

            Debug.Log($"장비 데이터 {equipmentList.Count}개를 EquipmentSO로 변환 완료!");
        }

        private void CreateEquipmentSO(EquipmentSO_DTO data)
        {
            EquipmentSO newEquipment = ScriptableObject.CreateInstance<EquipmentSO>();

            // 기본 정보 적용
            newEquipment.itemName = data.itemName;
            newEquipment.itemDescription = data.description;
            newEquipment.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), data.itemType);
            newEquipment.attack = data.attack;
            newEquipment.defense = data.defense;
            newEquipment.single_parentTAG = data.singleParentTag;

            // Vector3 데이터 변환
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
            // 생성한 EquipmentSO를 Assets 폴더에 저장
            string path = $"Assets/Resources/EquipmentSO/{data.itemName}.asset";
            AssetDatabase.CreateAsset(newEquipment, path);
            AssetDatabase.SaveAssets();

            // EquipmentSODatabase에 추가
            if (equipmentDatabase != null)
            {
                equipmentDatabase.equipmentSOs.Add(newEquipment);
                EditorUtility.SetDirty(equipmentDatabase);
            }
#endif
            Debug.Log($"EquipmentSO 생성 완료: {data.itemName}");
        }

    }

}
