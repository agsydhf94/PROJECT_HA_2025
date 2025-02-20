using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class EnemySpawner_Mission : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public List<GameObject> enemyRemaining = new List<GameObject>();
        public Transform[] spawnPoint;
        public int checkpointID;
        public bool isCompleted;
        // public int numberOfEnemies;

        private bool hasSpawned = false;
        private float lightDecreasingSpeed = 4f;

        public GameObject checkPointUI;
        public Color blinkingColor;




        private void OnTriggerEnter(Collider other)
        {
            if (!hasSpawned && other.CompareTag("Player"))
            {
                SpawnEnemies();
                hasSpawned = true;
            }
        }

        private void SpawnEnemies()
        {
            if(CheckPointManager.Instance.unlockedCheckPoints.Contains(checkpointID - 1) || checkpointID == 1)
            {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                var enemyInstance = Instantiate(enemyPrefab, spawnPoint[i].transform.position, Quaternion.identity);
                enemyRemaining.Add(enemyInstance);

                CharacterBase enemyCharacterBase = enemyInstance.GetComponent<CharacterBase>();
                    enemyCharacterBase.OnDeath += () =>
                    {
                        enemyRemaining.Remove(enemyInstance);
                        CheckAllEnemiesDefeated(checkpointID);
                    };

                SkinnedMeshRenderer enemy_skin = enemyInstance.GetComponentInChildren<SkinnedMeshRenderer>();
                Material enemy_material = enemy_skin.material;

                enemy_material.EnableKeyword("_EMISSION"); // Emission 활성화
                StartCoroutine(GlowEffect(enemy_material));
            }
            }
        }

        private IEnumerator GlowEffect(Material enemyMaterial)
        {
            float glowIntensity = 2f;
            while (glowIntensity > 0f)
            {
                glowIntensity -= Time.deltaTime * lightDecreasingSpeed; // 서서히 빛이 줄어듦
                enemyMaterial.SetColor("_EmissionColor", Color.red * glowIntensity);
                Debug.Log(glowIntensity);
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

        private void CheckAllEnemiesDefeated(int cpID)
        {
            if (enemyRemaining.Count == 0 && !isCompleted)
            {
                isCompleted = true;
                CheckPointManager.Instance.SaveCheckPoint(cpID);
                // checkPointUI.GetComponent<TextWriting>().StartUIAnimationSequence("CheckPoint Reached", blinkingColor);
                CheckPopUpUI.Instance.StartUIAnimationSequence("CheckPoint Reached", blinkingColor);
            }
        }
    }
}
