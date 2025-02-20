using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class UserDataModel : Singleton<UserDataModel>
    {
        [field: SerializeField] public PlayerDataDTO PlayerData { get; private set; }

        public void Initialize()
        {
            // For TEST - Save Player Data
            //SavePlayerData(new PlayerDataDTO()
            //{
            //    health = 100,
            //    level = 1,
            //    name = "REDFORCE",
            //    position = new Vector3(10, 0, 30)
            //});

            LoadPlayerData();

        }

        public void SavePlayerData(PlayerDataDTO data)
        {
            string playerDataToJson = JsonUtility.ToJson(data, true);
            Debug.Log(playerDataToJson);

            PlayerPrefs.SetString(typeof(PlayerDataDTO).Name, playerDataToJson);
        }

        public void LoadPlayerData()
        {
            string loadedPlayerDataString = PlayerPrefs.GetString(typeof(PlayerDataDTO).Name, string.Empty);
            Debug.Log(loadedPlayerDataString);

            PlayerData = JsonUtility.FromJson<PlayerDataDTO>(loadedPlayerDataString);
        }
    }
}
