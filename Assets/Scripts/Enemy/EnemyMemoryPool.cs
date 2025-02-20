using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


namespace HA
{
    public class EnemyMemoryPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemySpawnPointPrefab;
        [SerializeField]
        private GameObject enemyPrefab;
        [SerializeField]
        private float enemySpawnTime = 1.0f;  // �� ���� �ֱ�
        [SerializeField]
        private float enemySpawnLatency = 1.0f;  // Ÿ�� ���� �� ���� �����ϱ���� �ɸ��� �ð�

        private MemoryPool spawnPointMemoryPool;
        private MemoryPool enemyMemoryPool;

        private int numberOfEnemiesSpawnedAtOnce = 1;
        private Vector2Int mapSize = new Vector2Int(100, 100);

        private void Awake()
        {
            spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        }
    }
}
