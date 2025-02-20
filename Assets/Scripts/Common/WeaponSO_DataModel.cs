using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEditor;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HA
{
    public class WeaponSO_DataModel : Singleton<WeaponSO_DataModel>
    {

        /*
        public string jsonFolderName = "JsonData"; // JSON ������ ����� ����
        public string jsonFileName = "WeaponSOData.json"; // ��ȯ�� JSON ���ϸ�

        public void LoadWeaponData()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, jsonFolderName, jsonFileName);

            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"JSON ������ ã�� �� �����ϴ�: {jsonPath}");
                return;
            }

            string jsonData = File.ReadAllText(jsonPath);
            List<WeaponSO_DTO> weaponList = JsonConvert.DeserializeObject<List<WeaponSO_DTO>>(jsonData);

            foreach (var weapon in weaponList)
            {
                CreateWeaponSO(weapon);
            }

            Debug.Log($"Weapon ������ {weaponList.Count}���� WeaponSO�� ��ȯ �Ϸ�!");
        }

        private void CreateWeaponSO(WeaponSO_DTO data)
        {
            WeaponSO newWeapon = ScriptableObject.CreateInstance<WeaponSO>();

            // �⺻ ���� ����
            newWeapon.weaponName = data.weaponName;
            newWeapon.weaponCategory = (WeaponCategory)System.Enum.Parse(typeof(WeaponCategory), data.weaponCategory);
            newWeapon.enemyLayer = LayerMask.NameToLayer("Enemy");
            newWeapon.weaponDescription = data.weaponDescription;
            newWeapon.damagePoint = data.damagePoint;

            newWeapon.initial_bullets_InMagazine = data.initial_bullets_InMagazine;
            newWeapon.initial_bullets_Total = data.initial_bullets_Total;

            newWeapon.isSingleShot = data.isSingleShot;
            newWeapon.fireRate = data.fireRate;
            newWeapon.magazineCapacity = data.magazineCapacity;
            newWeapon.shootingRaycastMaxRange = data.shootingRaycastMaxRange;

            
            newWeapon.weaponSprite = Resources.Load<Sprite>($"Sprites/{data.weaponSpritePath}");
            newWeapon.bulletCasingPrefab = Resources.Load<GameObject>($"Prefabs/{data.bulletCasingPrefab}");
            newWeapon.muzzlePrefab = Resources.Load<GameObject>($"Prefabs/{data.muzzlePrefab}");
            newWeapon.bulletImpactParticle = Resources.Load<ParticleSystem>($"Effects/{data.bulletImpactParticle}");
            newWeapon.bulletTrailRenderer = Resources.Load<TrailRenderer>($"Effects/{data.bulletTrailRenderer}");
            newWeapon.fire_Sound = Resources.Load<AudioClip>($"Audio/{data.fire_Sound}");
            newWeapon.reload_Sound = Resources.Load<AudioClip>($"Audio/{data.reload_Sound}");

            // Transform Offset ����
            newWeapon.missileFire_Position = new Vector3(data.missileFire_PositionX, data.missileFire_PositionY, data.missileFire_PositionZ);
            newWeapon.missileFire_Rotation = new Vector3(data.missileFire_RotationX, data.missileFire_RotationY, data.missileFire_RotationZ);
            newWeapon.missileFire_Scale = new Vector3(data.missileFire_ScaleX, data.missileFire_ScaleY, data.missileFire_ScaleZ);

            newWeapon.fireTransform_Position = new Vector3(data.fireTransform_PositionX, data.fireTransform_PositionY, data.fireTransform_PositionZ);
            newWeapon.fireTransform_Rotation = new Vector3(data.fireTransform_RotationX, data.fireTransform_RotationY, data.fireTransform_RotationZ);
            newWeapon.fireTransform_Scale = new Vector3(data.fireTransform_ScaleX, data.fireTransform_ScaleY, data.fireTransform_ScaleZ);

            newWeapon.bulletCasingTransform_Position = new Vector3(data.bulletCasingTransform_PositionX, data.bulletCasingTransform_PositionY, data.bulletCasingTransform_PositionZ);
            newWeapon.bulletImpactTransform_Rotation = new Vector3(data.bulletImpactTransform_RotationX, data.bulletImpactTransform_RotationY, data.bulletImpactTransform_RotationZ);
            newWeapon.bulletImpactTransform_Scale = new Vector3(data.bulletImpactTransform_ScaleX, data.bulletImpactTransform_ScaleY, data.bulletImpactTransform_ScaleZ);

            newWeapon.katanaRange = data.katanaRange;
            newWeapon.swingVfx = LoadAddressableGameObject(data.swingVfxPath);

            // ScriptableObject ����
            string path = $"Assets/Resources/WeaponSO/{data.weaponName}.asset";
            AssetDatabase.CreateAsset(newWeapon, path);
            AssetDatabase.SaveAssets();


            Debug.Log($"WeaponSO ���� �Ϸ�: {data.weaponName}");
        }

        private void LoadAddressableAsset<T>(string key, System.Action<T> callback) where T : Object
        {
            if (string.IsNullOrEmpty(key)) return;
            Addressables.LoadAssetAsync<T>(key).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(handle.Result);
                }
                else
                {
                    Debug.LogError($"Addressable �ε� ����: {key}");
                }
            };
        }

        private GameObject LoadAddressableGameObject(string path)
        {
            return Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
        }
        */
        public string csvFileName = "Weapon_Data.csv"; // StreamingAssets ���� �� CSV ���ϸ�

        public void LoadWeaponData()
        {
            string csvPath = Path.Combine(Application.streamingAssetsPath, csvFileName);

            if (!File.Exists(csvPath))
            {
                Debug.LogError($"CSV ������ ã�� �� �����ϴ�: {csvPath}");
                return;
            }

            List<string[]> csvData = ReadCSV(csvPath);
            if (csvData.Count < 2)
            {
                Debug.LogError("CSV ������ ��� �ְų� �����Ͱ� �����ϴ�.");
                return;
            }

            // ù ��° ��(���)�� �����ϰ� ������ ��ȯ
            for (int i = 1; i < csvData.Count; i++)
            {
                CreateWeaponSO(csvData[i]);
            }

            Debug.Log($"Weapon ������ {csvData.Count - 1}���� WeaponSO�� ��ȯ �Ϸ�!");
        }

        private List<string[]> ReadCSV(string filePath)
        {
            List<string[]> data = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }

            return data;
        }

        private void CreateWeaponSO(string[] data)
        {
            WeaponSO newWeapon = ScriptableObject.CreateInstance<WeaponSO>();

            // �⺻ ���� ����
            newWeapon.weaponName = data[0];
            newWeapon.weaponCategory = (WeaponCategory)System.Enum.Parse(typeof(WeaponCategory), data[1]);
            newWeapon.enemyLayer = LayerMask.NameToLayer("Enemy");
            newWeapon.weaponDescription = data[2];
            newWeapon.weaponSprite = Resources.Load<Sprite>($"Sprites/{data[3]}");
            newWeapon.damagePoint = int.Parse(data[4]);

            newWeapon.initial_bullets_InMagazine = int.Parse(data[5]);
            newWeapon.initial_bullets_Total = int.Parse(data[6]);

            newWeapon.isSingleShot = bool.Parse(data[7]);
            newWeapon.fireRate = float.Parse(data[8]);
            newWeapon.magazineCapacity = int.Parse(data[9]);
            newWeapon.shootingRaycastMaxRange = float.Parse(data[10]);

            // ������ �� ����� ���ҽ� �ε�
            newWeapon.bulletCasingPrefab = Resources.Load<GameObject>($"Prefabs/{data[11]}");
            newWeapon.muzzlePrefab = Resources.Load<GameObject>($"Prefabs/{data[12]}");
            newWeapon.bulletImpactParticle = Resources.Load<ParticleSystem>($"Effects/{data[13]}");
            newWeapon.bulletTrailRenderer = Resources.Load<TrailRenderer>($"Effects/{data[14]}");
            newWeapon.bulletTrail_SpeedMultiplier = float.Parse(data[15]);
            newWeapon.fire_Sound = Resources.Load<AudioClip>($"Audio/{data[16]}");
            newWeapon.reload_Sound = Resources.Load<AudioClip>($"Audio/{data[17]}");

            // �̻��� �߻� ���� �Ӽ�
            newWeapon.missilePrefab = Resources.Load<GameObject>($"Prefabs/{data[18]}");
            newWeapon.targetingRange = float.Parse(data[19]);
            newWeapon.aimRadius = float.Parse(data[20]);

            // Transform Offset ����
            newWeapon.missileFire_Position = new Vector3(float.Parse(data[21]), float.Parse(data[22]), float.Parse(data[23]));
            newWeapon.missileFire_Rotation = new Vector3(float.Parse(data[24]), float.Parse(data[25]), float.Parse(data[26]));
            newWeapon.missileFire_Scale = new Vector3(float.Parse(data[27]), float.Parse(data[28]), float.Parse(data[29]));

            newWeapon.fireTransform_Position = new Vector3(float.Parse(data[30]), float.Parse(data[31]), float.Parse(data[32]));
            newWeapon.fireTransform_Rotation = new Vector3(float.Parse(data[33]), float.Parse(data[34]), float.Parse(data[35]));
            newWeapon.fireTransform_Scale = new Vector3(float.Parse(data[36]), float.Parse(data[37]), float.Parse(data[38]));

            newWeapon.bulletCasingTransform_Position = new Vector3(float.Parse(data[39]), float.Parse(data[40]), float.Parse(data[41]));
            newWeapon.bulletImpactTransform_Rotation = new Vector3(float.Parse(data[42]), float.Parse(data[43]), float.Parse(data[44]));
            newWeapon.bulletImpactTransform_Scale = new Vector3(float.Parse(data[45]), float.Parse(data[46]), float.Parse(data[47]));

            newWeapon.katanaRange = float.Parse(data[48]);
            newWeapon.swingVfx = Resources.Load<GameObject>($"Effects/{data[49]}");

#if UNITY_EDITOR
            // Resources ������ ScriptableObject ����
            string path = $"Assets/Resources/WeaponSO/{newWeapon.weaponName}";
            AssetDatabase.CreateAsset(newWeapon, path);
            AssetDatabase.SaveAssets();
#endif

            Debug.Log($"WeaponSO ���� �Ϸ�: {newWeapon.weaponName}");
        }
    }
}

