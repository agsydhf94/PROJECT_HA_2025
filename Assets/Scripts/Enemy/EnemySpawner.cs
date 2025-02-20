using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HA
{
    public class EnemySpawner : MonoBehaviour
    {
        public Enemy zombiePrefab;

        public EnemySO[] zombiedata_arr;
        public Transform[] spawnpoint_arr;

        private List<Enemy> zombies_list = new List<Enemy>();
        private int wave;

        private void Update()
        {
            if(GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                return;
            }

            if (zombies_list.Count <= 0)
            {
                SpawnWave();
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            IngameHUD.Instance.UpdateWaveText(wave, zombies_list.Count);
        }

        private void SpawnWave()
        {
            wave++;

            int spawnNumbers = Mathf.RoundToInt(wave * 1.5f);

            for(int i = 0; i < spawnNumbers; i++)
            {
                CreateZombie();
            }
        }

        private void CreateZombie()
        {
            EnemySO zombieData = zombiedata_arr[Random.Range(0, zombiedata_arr.Length)];

            Transform spawnPoint = spawnpoint_arr[Random.Range(0, spawnpoint_arr.Length)];

            Enemy zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

            zombie.InitialSetup(zombieData);

            zombies_list.Add(zombie);


            zombie.OnDeath += () => zombies_list.Remove(zombie);
            zombie.OnDeath += () => Destroy(zombie.gameObject, 10f);
            zombie.OnDeath += () => GameManager.Instance.AddScore(50);
        }
        
    }
}
