using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HA
{
    public class BootStrapper : MonoBehaviour
    {
        public static bool IsSystemLoaded { get; private set; } = false;

        public static string currentVaildSceneName;
        private static readonly List<string> AutoBootStrapperScenes = new List<string>()
        {
            "Main",
            "TitleScreen",
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SystemBoot()
        {
            IsSystemLoaded = false;

#if UNITY_EDITOR
            var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            for (int i = 0; i < AutoBootStrapperScenes.Count; i++)
            {
                if (activeScene.name.Equals(AutoBootStrapperScenes[i]))
                {
                    InternalBoot();
                    break;
                }
            }
#else
            InternalBoot();
#endif

            IsSystemLoaded = true;
        }

        private static void InternalBoot()
        {
            UIManager.Instance.Initialize();
            // SoundManager.Instance.Initialize();
            QuestManager.Instance.Initialize();

            QuestDataModel.Instance.Initialize();
            InventoryDataModel.Instance.Initialize();
            //GameDataModel.Singleton.Initialize();
            //UserDataModel.Singleton.Initialize();

            ExcelToJsonConverter.Instance.ConvertExcelToJson("EquipmentSOData.xlsx");
            ExcelToJsonConverter.Instance.ConvertExcelToJson("WeaponData.xlsx");
            //WeaponSO_DataModel.Instance.LoadWeaponData();

            //TODO : Add more system initialize

            var activeScene = SceneManager.GetActiveScene();
            bool isValidSceneName = false;
            foreach (var scene in AutoBootStrapperScenes)
            {
                if (activeScene.name.Equals(scene))
                {
                    isValidSceneName = true;
                    currentVaildSceneName = activeScene.name;
                    break;
                }
            }


            /*
            if (isValidSceneName)
            {
                switch (currentVaildSceneName)
                {
                    case "TitleScreen":
                        UIManager.Instance.Show<TitleUI>(UIList.TitleUI);
                        break;
                    case "InGameScene":
                        UIManager.Instance.Show<InteractionUI>(UIList.InteractionUI);
                        break;

                }
                //UIManager.Show<IngameUI>(UIList.IngameUI);
                
                //UIManager.Show<MinimapUI>(UIList.MinimapUI);
                //UIManager.Show<CrosshairUI>(UIList.CrosshairUI);

                // SoundManager.Singleton.PlayMusic(MusicFileName.BGM_02);
            }
            */
            if(isValidSceneName)
            {
                
                UIManager.Instance.Show<TitleUI>(UIList.TitleUI);
            }
        }
    }
}
